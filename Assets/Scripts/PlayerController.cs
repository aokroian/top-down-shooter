using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool alwaysAiming = false;

    public float rotationSpeed = 2f;
    public float basicMovementSpeed = 2f;
    public float dodgeSpeed = 20f;
    public float dodgeTime = 0.2f;
    public float dodgeStaminaCost = 20f;

    public float health = 100f;
    public float maxHealth = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenPerSecond = 5f;
    [Range(0.0f, 1.0f)]
    public float aimingEffectOnStaminaRegen = 1f;
    public int amountOfBullets = 1000;

    public GameObject[] weaponEquipmentArr;
    public int selectedWeaponIndex = 0;
    public int equippedWeaponIndex = 0;
    public GameObject parentBoneForWeapon;
    public GameObject aimSpotRef;
    public GameObject mainUIRef;

    public GameObject rightHandConstraintController;
    public GameObject leftHandConstraintController;
    public GameObject rigLayerHandsPosition;

    private float currentMovementSpeed = 0f;


    private GameObject equippedWeaponObj;

    public Vector3 mousePosOnGround;
    public Vector3 aimPosition;
    public Camera cam;

    public bool isAiming = false;

    public float minLookAtDistance = 1.0f;

    private Vector3 lookAt;
    private Vector2 movement;

    private GameObject rightHandBoneController;

    private float dodgeTimer = 0f;

    // constraint for animation rigging (weapon aim part)
    private GameObject weaponAimConstraintObj;

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
    private void Start()
    {
        isAiming = alwaysAiming;

        FindInAllChildren(gameObject.transform, "RightHandController", ref rightHandConstraintController);
        FindInAllChildren(gameObject.transform, "LeftHandController", ref leftHandConstraintController);
        FindInAllChildren(gameObject.transform, "AimWeapon", ref weaponAimConstraintObj);
        FindInAllChildren(gameObject.transform, "RigLayer_HandsPosition", ref rigLayerHandsPosition);

    }
    void Update()
    {

        if (GameLoopController.paused)
        {
            return;
        }


        // движение по плоскости
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        movement = Vector2.ClampMagnitude(movement, 1f);



        // floats for animation controller
        Vector3 globalMovement = new Vector3(movement.x, 0.0f, movement.y);
        Vector3 localMovement = gameObject.transform.InverseTransformDirection(globalMovement);
        gameObject.GetComponent<Animator>().SetFloat("local Z speed", localMovement.z);
        gameObject.GetComponent<Animator>().SetFloat("local X speed", localMovement.x);

        if (movement.magnitude <= 0.01f)
            gameObject.GetComponent<Animator>().SetBool("Is Idle", true);
        else
            gameObject.GetComponent<Animator>().SetBool("Is Idle", false);

        // hand on weapon animation rigging part
        // moving hand rig controllers to points on weapon when its equiped
        if (equippedWeaponObj != null && isAiming)
        {
            rightHandConstraintController.transform.position = equippedWeaponObj.transform.Find("RightHandPoint").transform.position;
            leftHandConstraintController.transform.position = equippedWeaponObj.transform.Find("LeftHandPoint").transform.position;
        }




        // controlling hand position constraints weight
        if (equippedWeaponObj == null)
        {
            rigLayerHandsPosition.GetComponent<Rig>().weight = 0f;
        }
        else
        {
            rigLayerHandsPosition.GetComponent<Rig>().weight = 1f;
        }



        /*
        // смещение
        Vector3 offset = new Vector3(movement.x, 0.0f, movement.y) * currentMovementSpeed;

        // игрок двигается в направлении курсора
        //transform.Translate(offset.normalized * currentMovementSpeed * Time.deltaTime);

        // игрок двигается по глобальным осям
        if (movement.magnitude >= 0.01f) {
            GetComponent<Rigidbody>().velocity = offset;
        } else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;

        }
            //transform.position += offset * Time.deltaTime;
        */

        // вращение персонажа 
        Vector3 lTargetDir = lookAt - transform.position;
        lTargetDir.y = 0.0f;

        float playerY = transform.rotation.eulerAngles.y;
        float directionY = Quaternion.LookRotation(lTargetDir).eulerAngles.y;

        // здесь, если рука отклонена слишком сильно, вращаем всего персонажа
        if (directionY - playerY >= 60f || directionY - playerY <= 1f)
        {
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lTargetDir), rotationSpeed));
        }

        // Clamp mouse position to screen size
        var mousePos = Input.mousePosition;
        var clampedMousePos = new Vector3(Mathf.Clamp(mousePos.x, 0f, Screen.width), Mathf.Clamp(mousePos.y, 0f, Screen.height), 0f);
        // расчеты для вращения персонажа туда, куда смотрит мышь
        Ray cameraRay = cam.ScreenPointToRay(clampedMousePos);


        // абстрактная поверхность для того, чтобы понять,
        // где луч из камеры пересекается с землей
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayLength;
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
            // определяем куда направлен персонаж
            lookAt = new Vector3(pointToLook.x, pointToLook.y, pointToLook.z);

            mousePosOnGround = lookAt;
            if (Vector3.Distance(lookAt, transform.position) < minLookAtDistance)
            {
                aimPosition = Vector3.Normalize(lookAt - transform.position) * minLookAtDistance;
            }
            else
            {
                aimPosition = lookAt;
            }
        }

        // регенерация стамины
        if (stamina < maxStamina)
        {
            if (isAiming)
                stamina += staminaRegenPerSecond * aimingEffectOnStaminaRegen * Time.deltaTime;
            else
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

        // примитивная механика уклонения
        if (Input.GetKeyDown(KeyCode.Space) && movement.magnitude > 0.01f && dodgeTimer == 0f && stamina >= dodgeStaminaCost)
        {
            dodgeTimer += dodgeTime;
            //transform.Find("Cube").GetComponent<SkinnedMeshRenderer>().enabled = false;
            //transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            stamina -= dodgeStaminaCost;

            gameObject.GetComponent<Animator>().SetBool("Is Dodging", true);
        }
        if (dodgeTimer > 0f)
        {
            dodgeTimer -= Time.deltaTime;
            currentMovementSpeed = dodgeSpeed;

        }
        else if (dodgeTimer == 0f)
        {
            //transform.Find("Cube").GetComponent<SkinnedMeshRenderer>().enabled = true;
            currentMovementSpeed = basicMovementSpeed;
            //transform.localScale = new Vector3(1f, 1f, 1f);
            gameObject.GetComponent<Animator>().SetBool("Is Dodging", false);
        }
        else if (dodgeTimer < 0f)
        {

            dodgeTimer = 0f;
        }

        // выбор и спавн оружия
        if (Input.GetKey(KeyCode.Alpha1))
        {
            selectedWeaponIndex = 1;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            selectedWeaponIndex = 2;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            selectedWeaponIndex = 0;
        }

        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Alpha3))
        {
            if (selectedWeaponIndex != equippedWeaponIndex && selectedWeaponIndex != 0)
            {
                if (equippedWeaponObj != null)
                {
                    Destroy(equippedWeaponObj);
                }
                // спавн оружия в качестве child правой руки
                equippedWeaponObj = Instantiate(weaponEquipmentArr[selectedWeaponIndex - 1], parentBoneForWeapon.transform);

                WeaponController weaponController = equippedWeaponObj.GetComponent<WeaponController>();

                weaponController.ownerObjRef = gameObject;
                // moving the weapon to the desired position
                equippedWeaponObj.transform.localPosition = weaponController.localPosition;
                equippedWeaponObj.transform.localRotation = Quaternion.Euler(weaponController.localRotation);
                equippedWeaponObj.transform.localScale = weaponController.localScale;

                // setup weapon aim constrained object (weapon obj)

                weaponAimConstraintObj.GetComponent<MultiAimConstraint>().data.constrainedObject = equippedWeaponObj.transform;
                gameObject.GetComponent<RigBuilder>().Build();

            }
        }

        // убрать экипированное оружие
        if (selectedWeaponIndex == 0)
        {
            Destroy(equippedWeaponObj);
        }

        // стрельба с предварительным прицеливанием
        //if (Input.GetMouseButton(1) && equippedWeaponObj != null)
        //{
        //    Vector3 pos = equippedWeaponObj.transform.Find("BulletOutPoint").gameObject.transform.position;
        //    Vector3 dir = mousePosOnGround + new Vector3(0.0f, 1.6f, 0.0f);
        //    Debug.DrawLine(pos, dir, Color.yellow);

        //    isAiming = true;

        //}
        //// стрельба без прицеливания
        //if (Input.GetMouseButton(0) && /* !isAiming &&*/  equippedWeaponObj != null)
        //{
        //    FindInAllChildren(transform, "AimAtPoint", ref rightHandBoneController);
        //    equippedWeaponObj.GetComponent<WeaponController>().Shoot(0, rightHandBoneController.transform.position);
        //}

        // shooting if always aiming
        if (Input.GetMouseButton(0) && equippedWeaponObj != null)
        {
            FindInAllChildren(transform, "AimAtPoint", ref rightHandBoneController);
            equippedWeaponObj.GetComponent<WeaponController>().Shoot(0, rightHandBoneController.transform.position);
        }

        // перезарядка
        if (Input.GetKeyDown(KeyCode.R) && equippedWeaponObj != null)
        {
            equippedWeaponObj.GetComponent<WeaponController>().Reload();
        }

        // Отмена прицеливания когда опускаем пкм
        //if (Input.GetMouseButtonUp(1))
        //{

        //    isAiming = alwaysAiming;
        //    aimSpotRef.gameObject.SetActive(false);
        //}
    }

    private void FixedUpdate()
    {
        // смещение
        Vector3 offset = new Vector3(movement.x, 0.0f, movement.y) * currentMovementSpeed;

        // игрок двигается в направлении курсора
        //transform.Translate(offset.normalized * currentMovementSpeed * Time.deltaTime);

        // игрок двигается по глобальным осям
        if (movement.magnitude >= 0.01f)
        {
            GetComponent<Rigidbody>().velocity = offset;
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;

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
        float result = 0f;
        if (equippedWeaponObj != null)
        {
            var weaponController = equippedWeaponObj.GetComponent<WeaponController>();
            result = weaponController.reloadTimer / weaponController.reloadTime;
        }
        return result;
    }

    public float GetBulletsInClip()
    {
        float result = 0f;
        if (equippedWeaponObj != null)
        {
            result = equippedWeaponObj.GetComponent<WeaponController>().bulletsInClip;
        }
        return result;
    }

    public float GetAmountOfBullets()
    {
        return amountOfBullets;
    }
}
