using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // variables for movement
    public float rotationSpeed = 2f;
    public float basicMovementSpeed = 2f;
    public float dodgeSpeed = 20f;
    public float dodgeTime = 0.2f;
    public float dodgeStaminaCost = 20f;

    private Vector2 wasdPosition;
    private Vector2 leftStickPosition;
    private bool allowedToDodge = false;
    private float currentMovementSpeed = 0f;
    private Vector2 movement;
    private float dodgeTimer = 0f;
    private Vector2 currentVel;

    // variables for health and stamina
    public float health = 100f;
    public float maxHealth = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenPerSecond = 5f;


    // variables for the inventory system
    public GameObject[] itemsEquipmentArr;
    public int selectedItemIndex = 0;
    public GameObject parentBoneForWeapon;
    public GameObject parentBoneForThrowableItems;
    public PlayerAmmoController ammoController;

    // Mobile controller
    public Joystick movementJoystick;
    public Joystick cameraJoystick;

    private GameObject equippedItemObj;
    private EquipmentItemType selectedItemType;


    // variables for animation rigging system
    public GameObject rightHandConstraintController;
    public GameObject leftHandConstraintController;
    public GameObject rigLayerHandsPosition;
    private GameObject weaponAimConstraintObj;

    private GameObject rightHandPoint;
    private GameObject leftHandPoint;

    // Should be reworked
    private bool grenadeInHand;


    // variables for aiming system
    public Camera cam;
    public Vector3 aimAtPosition;
    private Vector2 mousePosition;
    private Vector2 rightStickPosition;

    // other
    public PlayerInput playerInput;
    private string currentControlScheme;
    public GameObject gameLoop;

    private int[] bulletsInClip;
    private Animator animator;
    private Coroutine fireFrequency;
    private Coroutine shootingCoroutine;

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        leftStickPosition = Vector2.SmoothDamp(movement, inputMovement, ref currentVel, 0.0005f);
        wasdPosition = Vector2.SmoothDamp(movement, inputMovement, ref currentVel, 0.0005f);

    }
    public void OnAim(InputAction.CallbackContext value)
    {
        Vector2 inputAim = value.ReadValue<Vector2>();
        mousePosition = inputAim;
        rightStickPosition = inputAim;
    }
    public void OnShoot(InputAction.CallbackContext value)
    {
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
            if (value.performed)
            {
                SafelyStopShootingCoroutine();
                // single shot and only for strong weapons
                if (equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoType() == AmmoType.RIFLE)
                {
                    equippedItemObj.GetComponent<WeaponController>().Shoot(0);
                }
            }
        }
    }
    public void OnReload(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Reload();
        }
    }
    public void OnSwitchItem(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            if (selectedItemIndex + 1 == itemsEquipmentArr.Length)
            {
                SelectItem(0);
            }
            else
            {
                SelectItem(selectedItemIndex + 1);
            }
        }
    }
    public void OnDodge(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            allowedToDodge = true;
        }
    }
    public void OnPause()
    {
        if (GameLoopController.paused)
        {
            gameLoop.GetComponent<GameLoopController>().UnPause();
        }
        if (!GameLoopController.paused)
        {
            gameLoop.GetComponent<GameLoopController>().Pause();
        }

    }
    public void OnControlsChanged()
    {

        if (playerInput.currentControlScheme != currentControlScheme)
        {
            currentControlScheme = playerInput.currentControlScheme;
            RemoveAllBindingOverrides();
        }
    }
    void RemoveAllBindingOverrides()
    {
        InputActionRebindingExtensions.RemoveAllBindingOverrides(playerInput.currentActionMap);
    }


    private void Start()
    {
        FindInAllChildren(gameObject.transform, "RightHandController", ref rightHandConstraintController);
        FindInAllChildren(gameObject.transform, "LeftHandController", ref leftHandConstraintController);
        FindInAllChildren(gameObject.transform, "AimWeapon", ref weaponAimConstraintObj);
        FindInAllChildren(gameObject.transform, "RigLayer_HandsPosition", ref rigLayerHandsPosition);

        bulletsInClip = new int[itemsEquipmentArr.Length];
        for (int i = 0; i < bulletsInClip.Length; i++)
        {
            bulletsInClip[i] = -1;
        }

        // instantiating first item in inventory
        SelectItem(0);
        currentMovementSpeed = basicMovementSpeed;
        currentControlScheme = playerInput.currentControlScheme;
    }
    void Update()
    {
        if (GameLoopController.paused)
        {
            return;
        }

        // aiming and movement values update
        if (currentControlScheme == "Gamepad")
        {
            if (rightStickPosition.magnitude >= 0.05f)
            {
                Vector3 pos = new Vector3(rightStickPosition.x, 0f, rightStickPosition.y) * 2;
                aimAtPosition = transform.position + pos;


                // shooting for weak weapons
                if (equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoType() != AmmoType.RIFLE && shootingCoroutine == null)
                {
                    shootingCoroutine = StartCoroutine(Shooting());
                }
                else if (equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoType() == AmmoType.RIFLE)
                {
                    SafelyStopShootingCoroutine();
                }

            }
            if (rightStickPosition.magnitude < 0.05f)
            {
                // stop shooting
                SafelyStopShootingCoroutine();

                // keep the last direction
                aimAtPosition = transform.position + transform.forward * 2;
            }
            movement = leftStickPosition;
        }
        if (currentControlScheme == "Keyboard")
        {
            aimAtPosition = GetAimPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
            movement = wasdPosition;
        }
        if (currentControlScheme == "Touch")
        {
            //
        }

        // stamina system
        CalculateStamina();

        // animations
        animator = GetComponent<Animator>();
        Vector3 globalMovement = new Vector3(movement.x, 0.0f, movement.y);
        Vector3 localMovement = gameObject.transform.InverseTransformDirection(globalMovement);
        animator.SetFloat("local Z speed", localMovement.z);
        animator.SetFloat("local X speed", localMovement.x);

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


        // controlling hand position constraints weight
        if (equippedItemObj == null)
        {
            rigLayerHandsPosition.GetComponent<Rig>().weight = 0f;
        }
        else
        {
            rigLayerHandsPosition.GetComponent<Rig>().weight = 1f;
        }

        // dodge system РАБОТАЕТ ЧЕРЕЗ ЖОПУ
        if (allowedToDodge && movement.magnitude > 0.01f && dodgeTimer == 0f && stamina >= dodgeStaminaCost)
        {
            dodgeTimer += dodgeTime;
            stamina -= dodgeStaminaCost;

            animator.SetBool("Is Dodging", true);
        }
        if (dodgeTimer > 0f)
        {
            allowedToDodge = false;
            dodgeTimer -= Time.deltaTime;
            currentMovementSpeed = dodgeSpeed;

        }
        else if (dodgeTimer == 0f)
        {
            currentMovementSpeed = basicMovementSpeed;
            animator.SetBool("Is Dodging", false);
        }
        else if (dodgeTimer < 0f)
        {

            dodgeTimer = 0f;
        }
    }

    IEnumerator FireDelay()
    {
        while (true)
        {
            if (selectedItemType == EquipmentItemType.weapon)
            {
                if (equippedItemObj != null)
                {
                    equippedItemObj.GetComponent<WeaponController>().Shoot(0);
                }
            }
            if (Input.GetMouseButtonUp(0) && equippedItemObj != null && grenadeInHand)
            {
                if (equippedItemObj != null && grenadeInHand)
                {
                    // direction and force
                    Vector3 upForce = new Vector3(0f, 4f, 0f);
                    Vector3 throwForce = (aimAtPosition - transform.position) + gameObject.GetComponent<Rigidbody>().velocity + upForce;

                    equippedItemObj.GetComponent<ThrowableItemController>().Throw(throwForce);
                    selectedItemIndex = 0;
                    equippedItemObj = null;

                    grenadeInHand = false;

                    if (ammoController.HasAmmo(AmmoType.GRENADE))
                    {
                        SelectItem(3);
                    }
                    /*else
                    {
                        SelectItem(0);
                    }*/
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Shooting()
    {
        while (true)
        {
            equippedItemObj.GetComponent<WeaponController>().Shoot(0);
            yield return new WaitForFixedUpdate();
        }
    }
    private void SafelyStopShootingCoroutine()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    public void StartShooting()
    {
        fireFrequency = StartCoroutine(FireDelay());
    }

    public void StopShooting()
    {
        StopCoroutine(fireFrequency);
    }

    public void Reload()
    {
        if (equippedItemObj != null)
        {
            equippedItemObj.GetComponent<WeaponController>().Reload();
        }
    }

    public void NextWeapon()
    {

    }
    public void Dodge()
    {

    }

    private void FixedUpdate()
    {
        // смещение
        Vector3 offset = new Vector3(movement.x, 0.0f, movement.y) * currentMovementSpeed;

        // игрок двигается по глобальным осям
        if (movement.magnitude >= 0.01f)
        {
            GetComponent<Rigidbody>().velocity = offset;
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        // вращение персонажа 
        Vector3 lTargetDir = aimAtPosition - transform.position;
        lTargetDir.y = 0f;

        GetComponent<Rigidbody>().MoveRotation(Quaternion.LookRotation(lTargetDir, Vector3.up));
    }

    private void SelectItem(int itemIndex)
    {
        if (equippedItemObj != null)
        {
            bulletsInClip[selectedItemIndex] = equippedItemObj.GetComponent<IAmmoConsumer>().GetAmmoLeft();
            Destroy(equippedItemObj);
        }

        selectedItemIndex = itemIndex;




        equippedItemObj = Instantiate(itemsEquipmentArr[itemIndex], parentBoneForWeapon.transform);
        WeaponController weaponController = equippedItemObj.GetComponent<WeaponController>();

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

    private void CalculateStamina()
    {
        if (stamina < maxStamina)
        {
            stamina += staminaRegenPerSecond * Time.deltaTime;
        }
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
        return (100 - stamina) / 100f;
    }

    public float GetReloadTimerPercent()
    {
        if (selectedItemType == EquipmentItemType.weapon)
        {
            float result = 0f;
            if (equippedItemObj != null)
            {
                var weaponController = equippedItemObj.GetComponent<WeaponController>();
                result = weaponController.reloadTimer / weaponController.reloadTime;
            }
            return result;
        }
        else
        {
            return 0f;
        }

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

    private Vector3 GetJoystickRotation()
    {
        return new Vector3(
            transform.position.x + cameraJoystick.Horizontal * 10,
            0,
            transform.position.z + cameraJoystick.Vertical * 10);
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
}
