using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    // variables for movement
    public float basicMovementSpeed = 2f;
    public float dodgeSpeed = 20f;
    public float dodgeStaminaCostPerSec = 20f;
    public float dodgeAnimationMultiplier = 2;

    private Vector2 wasdPosition;
    private Vector2 leftStickPosition;
    private bool allowedToDodge = false;
    private float currentMovementSpeed = 0f;
    private Vector2 movement;

    // variables for health and stamina
    public float health = 100f;
    public float maxHealth = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenPerSecond = 5f;


    // variables for the inventory system
    private GameObject[] itemsEquipmentArr;
    public int selectedItemIndex = 0;
    public GameObject parentBoneForWeapon;
    public PlayerAmmoController ammoController;
    public IngameProgressionManager progressionManager;
    private GameObject equippedItemObj;
    WeaponController weaponController;
    private bool needToSelectPistol = false;

    // Touch controls
    public GameObject mobileInput;

    // variables for animation rigging system
    [HideInInspector]
    public GameObject rightHandConstraintController;
    [HideInInspector]
    public GameObject leftHandConstraintController;

    private GameObject rightHandPoint;
    private GameObject leftHandPoint;

    // variables for aiming system
    public Camera cam;
    public Vector3 aimAtPosition;
    private Vector2 mousePosition;
    private Vector2 rightStickPosition;
    // test 
    private Vector2 lastRightStickPosition;
    private bool rifleShootNeeded = false;

    // other
    public PlayerInput playerInput;
    private string currentControlScheme;
    public GameObject gameLoop;

    private int[] bulletsInClip;
    private Animator animator;
    private Coroutine shootingCoroutine;
    // area attack
    private AreaAttack areaAttack;

    // upgrades
    private PlayerConfigurator playerConfigurator;

    //sound
    private PlayerAudioManager playerAudioManager;
    private AudioSource movementAudioSource;

    public float distance { get; private set; }

    public void OnMovement(InputAction.CallbackContext value)
    {
        if (GameLoopController.paused)
        {
            return;
        }
        if (value.performed)
        {
            Vector2 inputMovement = value.ReadValue<Vector2>();
            Vector3 toConvert = new Vector3(inputMovement.x, 0, inputMovement.y);
            Vector3 converted = IsoVectorConvert(toConvert);
            leftStickPosition = new Vector2(converted.x, converted.z);
            wasdPosition = new Vector2(converted.x, converted.z);
        }
        else if (value.canceled)
        {
            Vector2 zeroMovement = Vector2.zero;
            leftStickPosition = zeroMovement;
            wasdPosition = zeroMovement;
        }
    }
    public void OnAim(InputAction.CallbackContext value)
    {
        if (GameLoopController.paused)
        {
            return;
        }
        if (value.performed)
        {
            Vector2 inputAim = value.ReadValue<Vector2>();
            Vector3 toConvert = new Vector3(inputAim.x, 0, inputAim.y);
            Vector3 converted = IsoVectorConvert(toConvert);
            mousePosition = new Vector2(converted.x, converted.z);
            rightStickPosition = new Vector2(converted.x, converted.z);
            if (lastRightStickPosition != rightStickPosition)
            {
                lastRightStickPosition = rightStickPosition;
            }
        }
        else if (value.canceled)
        {
            Vector2 zeroMovement = Vector2.zero;
            mousePosition = zeroMovement;
            rightStickPosition = zeroMovement;
        }
    }
    public void OnShoot(InputAction.CallbackContext value)
    {
        if (GameLoopController.paused)
        {
            return;
        }
        // keyboard
        if (currentControlScheme == "Keyboard")
        {
            if (value.performed)
            {
                SafelyStopShootingCoroutine();
                shootingCoroutine = StartCoroutine(Shooting());
            }
            if (value.canceled)
            {
                SafelyStopShootingCoroutine();
            }
        }

        // gamepad 
        if (currentControlScheme == "Gamepad")
        {
            if (GameLoopController.paused)
            {
                return;
            }
            if (value.performed)
            {
                SafelyStopShootingCoroutine();
                // single shot and only for strong weapons
                if (equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoType() == AmmoType.RIFLE)
                {
                    equippedItemObj.transform.GetComponentInChildren<LaserAim>().isEnabled = false;
                    SingleShot();
                    SafelyStopShootingCoroutine();
                }
            }
        }
    }
    public void OnReload(InputAction.CallbackContext value)
    {
        if (GameLoopController.paused)
        {
            return;
        }
        if (value.performed)
        {
            if (equippedItemObj != null)
            {
                equippedItemObj.GetComponent<WeaponController>().Reload();
            }
        }
    }
    public void OnSwitchItem(InputAction.CallbackContext value)
    {
        if (GameLoopController.paused)
        {
            return;
        }
        SafelyStopShootingCoroutine();
        if (value.performed)
        {
            // sound 
            playerAudioManager.PlaySwitchWeaponSound();

            // item
            int NextItemIndex()
            {
                int result = selectedItemIndex + 1 == itemsEquipmentArr.Length ? 0 : selectedItemIndex + 1;
                return result;
            }

            SelectItem(NextItemIndex());
            // if no ammo in next weapon, select next to it
            while (!CheckAmmo(weaponController))
            {
                SelectItem(NextItemIndex());
            }
        }
    }
    public void OnDodge(InputAction.CallbackContext value)
    {
        if (GameLoopController.paused)
        {
            return;
        }
        if (value.performed)
        {
            allowedToDodge = true;
        }
        if (value.canceled)
        {
            if (allowedToDodge) allowedToDodge = false;
        }
    }
    public void OnPause(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            if (GameLoopController.paused)
            {
                gameLoop.GetComponent<GameLoopController>().UnPause();
            }
            else if (!GameLoopController.paused)
            {
                gameLoop.GetComponent<GameLoopController>().Pause();
            }
        }

    }
    public void OnControlsChanged()
    {
        if (playerInput.currentControlScheme != currentControlScheme)
        {

            currentControlScheme = playerInput.currentControlScheme;
            switch (currentControlScheme)
            {
                case "Keyboard":
                    break;
                case "Gamepad":
                    break;
                case "Touch":
                    break;
            }
            Debug.Log("Changed: " + currentControlScheme);
            RemoveAllBindingOverrides();
        }
    }
    void RemoveAllBindingOverrides()
    {
        InputActionRebindingExtensions.RemoveAllBindingOverrides(playerInput.currentActionMap);
    }

    private void Awake()
    {
        itemsEquipmentArr = progressionManager.GetWeapons();
    }

    private void OnEnable()
    {
        playerInput.SwitchCurrentControlScheme("Gamepad");

        RemoveAllBindingOverrides();
    }

    private void Start()
    {
        // upgrade system
        TryGetComponent<PlayerConfigurator>(out playerConfigurator);
        if (playerConfigurator != null)
        {
            playerConfigurator.ApplyAllUpgrades();
        }
        // sound system
        movementAudioSource = transform.Find("MovementAudioSource").GetComponent<AudioSource>();
        TryGetComponent<PlayerAudioManager>(out playerAudioManager);

        // finding objects for rigging
        FindInAllChildren(gameObject.transform, "RightHandController", ref rightHandConstraintController);
        FindInAllChildren(gameObject.transform, "LeftHandController", ref leftHandConstraintController);

        bulletsInClip = new int[itemsEquipmentArr.Length];
        for (int i = 0; i < bulletsInClip.Length; i++)
        {
            bulletsInClip[i] = -1;
        }

        // instantiating first item in inventory
        SelectItem(0);
        weaponController = equippedItemObj.GetComponent<WeaponController>();
        currentMovementSpeed = basicMovementSpeed;
        currentControlScheme = playerInput.currentControlScheme;

        // area attack
        TryGetComponent<AreaAttack>(out areaAttack);

#if UNITY_ANDROID || UNITY_IOS
        mobileInput.SetActive(true);
#endif
#if UNITY_STANDALONE
        mobileInput.SetActive(false);
#endif
    }

    void Update()
    {
        distance = Vector3.Distance(Vector3.zero, transform.position);
        if (GameLoopController.paused)
        {
            movementAudioSource.clip = null;
            movementAudioSource.loop = false;
            movementAudioSource.pitch = 1;
            movementAudioSource.volume = 1;
            movementAudioSource.Stop();
            return;
        }
        Rigidbody rb = GetComponent<Rigidbody>();

        // run sound
        if (rb.velocity.magnitude >= 1)
        {
            playerAudioManager.PlayStepsSound(true);
        }
        else if (rb.velocity.magnitude < 1)
        {
            playerAudioManager.PlayStepsSound(false);
        }

        // aiming and movement values update
        if (currentControlScheme == "Gamepad" || currentControlScheme == "Touch")
        {
            if (rightStickPosition.magnitude >= 0.2f)
            {
                Vector3 pos = new Vector3(rightStickPosition.normalized.x, 0f, rightStickPosition.normalized.y) * 12;
                aimAtPosition = transform.position + pos;


                if (rightStickPosition.magnitude >= 0.7f)
                {
                    // shooting for weak weapons
                    if (equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoType() != AmmoType.RIFLE && shootingCoroutine == null)
                    {
                        shootingCoroutine = StartCoroutine(Shooting());
                    }
                    else if (equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoType() == AmmoType.RIFLE)
                    {
                        SafelyStopShootingCoroutine();
                        equippedItemObj.transform.GetComponentInChildren<LaserAim>().isEnabled = true;
                    }
                }
                if (rightStickPosition.magnitude < 0.7f)
                {
                    // stop shooting
                    SafelyStopShootingCoroutine();
                }
            }
            if (rightStickPosition.magnitude < 0.2f)
            {
                // stop shooting
                SafelyStopShootingCoroutine();
                // keep the last direction
                aimAtPosition = transform.position + transform.forward * 12;
            }


            movement = leftStickPosition;
        }
        if (currentControlScheme == "Keyboard")
        {
            aimAtPosition = GetAimPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
            movement = wasdPosition;
        }

        // animations
        animator = GetComponent<Animator>();
        Vector3 globalMovement = new Vector3(movement.x, 0.0f, movement.y);
        Vector3 localMovement = gameObject.transform.InverseTransformDirection(globalMovement);
        if (rb.velocity.magnitude >= 0.1f)
        {
            animator.SetFloat("local Z speed", localMovement.z);
            animator.SetFloat("local X speed", localMovement.x);
        }
        else if (rb.velocity.magnitude < 0.1f)
        {
            animator.SetFloat("local Z speed", 0);
            animator.SetFloat("local X speed", 0);
        }


        // hand on item animation rigging part
        // moving hand rig controllers to points on weapon when its equiped
        if (rightHandPoint != null)
        {
            rightHandConstraintController.transform.position = rightHandPoint.transform.position;
        }
        if (leftHandPoint != null)
        {
            leftHandConstraintController.transform.position = leftHandPoint.transform.position;
        }

        // dodge system
        // stamina system
        StaminaCorrection();
        if (allowedToDodge && stamina > 0)
        {
            areaAttack.isEnabled = true;
            

            stamina -= dodgeStaminaCostPerSec * Time.deltaTime;
            currentMovementSpeed = dodgeSpeed;

            animator.SetFloat("run multiplier", dodgeAnimationMultiplier);

            // stop walking audio
            playerAudioManager.PlayAreaAttackSound(true);
            playerAudioManager.PlayStepsSound(false);

        }
        else if (stamina <= 0 || !allowedToDodge)
        {
            if (stamina < maxStamina)
            {
                stamina += staminaRegenPerSecond * Time.deltaTime;
            }
            
            areaAttack.isEnabled = false;
            allowedToDodge = false;
            animator.SetFloat("run multiplier", 1);
            currentMovementSpeed = basicMovementSpeed;

            // continue walking audio
            playerAudioManager.PlayAreaAttackSound(false);
            playerAudioManager.PlayStepsSound(true);
        }
    }

    private void FixedUpdate()
    {
        // shooting for strong weapons
        if (equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoType() == AmmoType.RIFLE)
        {
            if (lastRightStickPosition.magnitude - rightStickPosition.magnitude >= 0.1f && !rifleShootNeeded)
            {
                rifleShootNeeded = true;
            }
            if (rightStickPosition.magnitude <= 0.3f && rifleShootNeeded)
            {
                equippedItemObj.transform.GetComponentInChildren<LaserAim>().isEnabled = false;
                SafelyStopShootingCoroutine();
                SingleShot();
                rifleShootNeeded = false;
                lastRightStickPosition = rightStickPosition;
            }

        }


        // смещение
        Vector3 offset = new Vector3(movement.x, -3.4f, movement.y) * currentMovementSpeed;
        Rigidbody rb = GetComponent<Rigidbody>();

        // игрок двигается по глобальным осям
        if (movement.magnitude >= 0.01f)
        {
            rb.velocity = offset;
        }
        else
        {
            rb.velocity = new Vector3(0f, -3.4f, 0f);
        }

        // вращение персонажа 
        Vector3 lTargetDir = aimAtPosition - transform.position;
        lTargetDir.y = 0f;


        rb.MoveRotation(Quaternion.LookRotation(lTargetDir, Vector3.up));
    }

    IEnumerator Shooting()
    {
        while (true)
        {
            SingleShot();
            yield return new WaitForFixedUpdate();
        }
    }

    private void SingleShot()
    {
        
        if (!CheckAmmo(weaponController))
        {
            if (needToSelectPistol)
            {
                SelectItem(0);
                needToSelectPistol = false;
            }
            // audio
            weaponController.PlayNoBulletSound();
            needToSelectPistol = true;
        } else
        {
            weaponController.Shoot(0);
        }
    }

    private bool CheckAmmo(WeaponController weaponController)
    {
        bool result = true;
        if (!weaponController.ammoProvider.HasAmmo(weaponController.ammoType) && weaponController.bulletsInClip <= 0)
        {
            result = false;
        }
        return result;
    }

    private void SafelyStopShootingCoroutine()
    {
        if (equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoType() == AmmoType.RIFLE)
        {
            equippedItemObj.transform.GetComponentInChildren<LaserAim>().isEnabled = false;
        }

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }
 
    private void SelectItem(int itemIndex)
    {
        if (equippedItemObj != null)
        {
            bulletsInClip[selectedItemIndex] = equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoLeft();
            Destroy(equippedItemObj);
        }

        selectedItemIndex = itemIndex;

        rightStickPosition = Vector2.zero;
        equippedItemObj = Instantiate(itemsEquipmentArr[itemIndex], parentBoneForWeapon.transform);
        weaponController = equippedItemObj.GetComponent<WeaponController>();

        weaponController.ownerObjRef = gameObject;
        weaponController.ammoProvider = ammoController;
        RestoreWeaponBullets(weaponController, itemIndex);
        ammoController.currentWeapon = weaponController;
        // moving the weapon to the desired position
        equippedItemObj.transform.localPosition = weaponController.localPosition;
        equippedItemObj.transform.localRotation = Quaternion.Euler(weaponController.localRotation);
        equippedItemObj.transform.localScale = weaponController.localScale;
        // updating hand points on item
        FindInAllChildren(equippedItemObj.transform, "RightHandPoint", ref rightHandPoint);
        FindInAllChildren(equippedItemObj.transform, "LeftHandPoint", ref leftHandPoint);

        weaponController.noShootingTime = 1;

        // UPGRADE SYSTEM
        // change ammo capacity


        progressionManager.WeaponChanged(itemsEquipmentArr[itemIndex]);
    }

    private Vector3 GetAimPoint(Vector3 mousePosition)
    {
        // Clamp mouse position to screen size
        Vector3 clampedMousePos = new Vector3(Mathf.Clamp(mousePosition.x, 0f, Screen.width), Mathf.Clamp(mousePosition.y, 0f, Screen.height), 0f);
        Ray cameraRay = cam.ScreenPointToRay(clampedMousePos);
        // абстрактная поверхность для того, чтобы понять,
        // где луч из камеры пересекается с землей
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 1.57f, 0f));
        float rayLength;
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            return cameraRay.GetPoint(rayLength); ;
        }
        else
        {
            return aimAtPosition;
        }
    }

    private void StaminaCorrection()
    {
        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
        if (stamina < 0f)
        {
            stamina = 0f;
        }
    }

    public float GetHealthPercent()
    {
        return GetComponent<Target>().health / GetComponent<Target>().maxHealth;
    }

    public float GetStaminaPercent()
    {
        return stamina / maxStamina;
    }

    public float GetReloadTimerPercent()
    {

        float result = 0f;
        if (equippedItemObj != null)
        {
            var weaponController = equippedItemObj.GetComponent<WeaponController>();
            result = weaponController.reloadTimer / weaponController.reloadTime;
        }
        return result;
    }

    private void RestoreWeaponBullets(WeaponController weapon, int index)
    {
        if (bulletsInClip[index] != -1)
        {
            weapon.bulletsInClip = bulletsInClip[index];
        }
    }

    public AmmoType[] GetAmmoTypes()
    {
        AmmoType[] result = new AmmoType[itemsEquipmentArr.Length];

        for (int i = 0; i < itemsEquipmentArr.Length; i++)
        {
            result[i] = itemsEquipmentArr[i].GetComponent<IAmmoConsumer>().GetAmmoType();
        }

        return result;
    }

    private void FindInAllChildren(Transform obj, string name, ref GameObject storeInObj)
    {
        if (obj.Find(name) != null)
        {
            storeInObj = obj.Find(name).gameObject;
        }
        else
        {
            foreach (Transform eachChild in obj)
            {
                if (eachChild.childCount > 0)
                {
                    FindInAllChildren(eachChild, name, ref storeInObj);
                }
            }
        }
    }
    // converting input Vector2 for isometric camera with 45 degrees 
    private Vector3 IsoVectorConvert(Vector3 vector)
    {
        Quaternion rotation = Quaternion.Euler(0, 45f, 0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
        Vector3 result = isoMatrix.MultiplyPoint3x4(vector);
        return result;
    }

    public bool AddEnergy(float amount)
    {
        bool result = false;
        if (stamina < maxStamina)
        {
            stamina += amount;
            result = true;
        }
        return result;
    }

    public bool CanAddEnergy()
    {
        return stamina < maxStamina;
    }
}
