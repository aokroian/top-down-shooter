using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // variables for movement
    public float rotationSpeed = 2f;
    public float basicMovementSpeed = 2f;
    public float dodgeSpeed = 20f;
    public float dodgeTime = 0.2f;
    public float dodgeStaminaCost = 20f;

    private float currentMovementSpeed = 0f;
    private Vector2 movement;
    private float dodgeTimer = 0f;

    // variables for health and stamina
    public float health = 100f;
    public float maxHealth = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenPerSecond = 5f;


    // variables for the inventory system
    public GameObject[] itemsEquipmentArr;
    public int selectedItemIndex = 0;
    public int equippedItemIndex = 0;
    public GameObject parentBoneForWeapon;
    public GameObject parentBoneForThrowableItems;
    public PlayerAmmoController ammoController;
    //public int amountOfBullets = 1000;
    //public int amountOfGrenades = 1000;

    private GameObject equippedItemObj;
    private EquipmentItemType selectedItemType;


    // variables for animation rigging system
    public GameObject rightHandConstraintController;
    public GameObject leftHandConstraintController;
    public GameObject rigLayerHandsPosition;

    private GameObject aimAtPointController;
    private GameObject weaponAimConstraintObj;

    private GameObject rightHandPoint;
    private GameObject leftHandPoint;

    // Should be reworked
    private bool grenadeInHand;


    // variables for aiming system
    public Camera cam;
    public float minLookAtDistance = 1.0f;
    public Vector3 aimAtPosition;

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

        // rewrite variables for movement 
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        movement = Vector2.ClampMagnitude(movement, 1f);
        // set aiming point from mouse on screen position (for PC)
        aimAtPosition = GetAimPoint(Input.mousePosition);

        // stamina system
        CalculateStamina();




        // animations
        Vector3 globalMovement = new Vector3(movement.x, 0.0f, movement.y);
        Vector3 localMovement = gameObject.transform.InverseTransformDirection(globalMovement);
        gameObject.GetComponent<Animator>().SetFloat("local Z speed", localMovement.z);
        gameObject.GetComponent<Animator>().SetFloat("local X speed", localMovement.x);

        if (movement.magnitude <= 0.01f)
            gameObject.GetComponent<Animator>().SetBool("Is Idle", true);
        else
            gameObject.GetComponent<Animator>().SetBool("Is Idle", false);

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

        // dodge system
        if (Input.GetKeyDown(KeyCode.Space) && movement.magnitude > 0.01f && dodgeTimer == 0f && stamina >= dodgeStaminaCost)
        {
            dodgeTimer += dodgeTime;
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
            currentMovementSpeed = basicMovementSpeed;
            gameObject.GetComponent<Animator>().SetBool("Is Dodging", false);
        }
        else if (dodgeTimer < 0f)
        {

            dodgeTimer = 0f;
        }

        // item selection system
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectItem(0);
        }


        // shooting
        if (selectedItemType == EquipmentItemType.weapon)
        {
            if (Input.GetMouseButton(0) && equippedItemObj != null)
            {
                FindInAllChildren(transform, "AimAtPoint", ref aimAtPointController);
                equippedItemObj.GetComponent<WeaponController>().Shoot(0, aimAtPointController.transform.position);
            }

            // reload equipped weapon
            if (Input.GetKeyDown(KeyCode.R) && equippedItemObj != null)
            {
                equippedItemObj.GetComponent<WeaponController>().Reload();
            }
        }


        // TODO: Shouldnt be here !! At throwableItemController?
        // throwing grenades
        if (selectedItemType == EquipmentItemType.throwableItem)
        {
            if (Input.GetMouseButtonUp(0) && equippedItemObj != null && grenadeInHand)
            {
                FindInAllChildren(transform, "AimAtPoint", ref aimAtPointController);

                // direction and force
                Vector3 upForce = new Vector3(0f, 3f, 0f);
                Vector3 throwForce = (aimAtPosition - transform.position) + gameObject.GetComponent<Rigidbody>().velocity + upForce;

                equippedItemObj.GetComponent<ThrowableItemController>().Throw(throwForce);
                selectedItemIndex = 0;
                equippedItemIndex = 0;
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

        // вращение персонажа 
        Vector3 lTargetDir = aimAtPosition - transform.position;
        lTargetDir.y = 0.0f;

        float playerY = transform.rotation.eulerAngles.y;
        float directionY = Quaternion.LookRotation(lTargetDir).eulerAngles.y;

        // здесь, если рука отклонена слишком сильно, вращаем всего персонажа
        if (directionY - playerY >= 60f || directionY - playerY <= 1f)
        {
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lTargetDir), rotationSpeed));
        }

    }

    private void SelectItem(int itemIndex)
    {
        // remove item if needed
        if (itemIndex == 0)
        {
            Destroy(equippedItemObj);
            return;
        }

        if (selectedItemIndex != itemIndex)
        {
            if (equippedItemObj != null)
            {
                Destroy(equippedItemObj);
            }

            selectedItemType = itemsEquipmentArr[itemIndex - 1].GetComponent<EquipmentItemController>().itemType;

            if (selectedItemType == EquipmentItemType.weapon)
            {
                equippedItemObj = Instantiate(itemsEquipmentArr[itemIndex - 1], parentBoneForWeapon.transform);
                WeaponController weaponController = equippedItemObj.GetComponent<WeaponController>();

                weaponController.ownerObjRef = gameObject;
                weaponController.ammoProvider = ammoController;
                ammoController.currentWeapon = weaponController;
                // moving the weapon to the desired position
                equippedItemObj.transform.localPosition = weaponController.localPosition;
                equippedItemObj.transform.localRotation = Quaternion.Euler(weaponController.localRotation);
                equippedItemObj.transform.localScale = weaponController.localScale;
                // setup weapon aim constrained object (weapon obj)
                weaponAimConstraintObj.GetComponent<MultiAimConstraint>().data.constrainedObject = equippedItemObj.transform;
                gameObject.GetComponent<RigBuilder>().Build();
            }
            else if (selectedItemType == EquipmentItemType.throwableItem)
            {
                if (grenadeInHand || ammoController.HasAmmo(AmmoType.GRENADE))
                {
                    if (!grenadeInHand)
                    {
                        ammoController.GetAmmo(AmmoType.GRENADE, 1);
                        grenadeInHand = true;
                    }
                    equippedItemObj = Instantiate(itemsEquipmentArr[itemIndex - 1], parentBoneForThrowableItems.transform);
                    ThrowableItemController throwableItemController = equippedItemObj.GetComponent<ThrowableItemController>();

                    throwableItemController.ownerObjRef = gameObject;
                    throwableItemController.ammoProvider = ammoController;
                    ammoController.currentWeapon = throwableItemController;
                    // moving the weapon to the desired position
                    equippedItemObj.transform.localPosition = throwableItemController.localPosition;
                    equippedItemObj.transform.localRotation = Quaternion.Euler(throwableItemController.localRotation);
                    equippedItemObj.transform.localScale = throwableItemController.localScale;

                } else
                {
                    SelectItem(0);
                    return;
                }
            }

            // updating hand points on item
            FindInAllChildren(equippedItemObj.transform, "RightHandPoint", ref rightHandPoint);
            FindInAllChildren(equippedItemObj.transform, "LeftHandPoint", ref leftHandPoint);

        }

        selectedItemIndex = itemIndex;
    }

    private Vector3 GetAimPoint(Vector3 mousePosition)
    {
        // Clamp mouse position to screen size
        Vector3 clampedMousePos = new Vector3(Mathf.Clamp(mousePosition.x, 0f, Screen.width), Mathf.Clamp(mousePosition.y, 0f, Screen.height), 0f);
        Ray cameraRay = cam.ScreenPointToRay(clampedMousePos);
        // абстрактная поверхность для того, чтобы понять,
        // где луч из камеры пересекается с землей
        Plane groundPlane = new Plane(new Vector3 (0f, 1.57f, 0f), Vector3.zero);
        float rayLength;
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            //Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);

            if (Vector3.Distance(pointToLook, transform.position) < minLookAtDistance)
            {
                return Vector3.Normalize(pointToLook - transform.position) * minLookAtDistance;
            }
            else
            {
                return pointToLook;
            }
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
}
