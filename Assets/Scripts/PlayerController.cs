using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public GameObject rightHandBoneRef;
    public GameObject aimSpotRef;
    public GameObject mainUIRef;


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


        // смещение
        Vector3 offset = new Vector3(movement.x, 0.0f, movement.y) * currentMovementSpeed;

        // игрок двигается в направлении курсора
        //transform.Translate(offset.normalized * currentMovementSpeed * Time.deltaTime);

        // игрок двигается по глобальным осям
        if (movement.magnitude >= 0.01f)
            transform.position += offset * Time.deltaTime;

        // вращение персонажа 
        Vector3 lTargetDir = lookAt - transform.position;
        lTargetDir.y = 0.0f;

        float playerY = transform.rotation.eulerAngles.y;
        float directionY = Quaternion.LookRotation(lTargetDir).eulerAngles.y;

        // здесь, если рука отклонена слишком сильно, вращаем всего персонажа
        if (directionY - playerY >= 20f || directionY - playerY <= -10f)
        {
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lTargetDir), rotationSpeed);
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
        //  обновление значения стамины в UI
        mainUIRef.transform.Find("StaminaScrollbar").gameObject.GetComponent<Scrollbar>().value = (100 - stamina) / 100;
        mainUIRef.transform.Find("StaminaScrollbar").gameObject.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, stamina / 100);
        // примитивная механика уклонения
        if (Input.GetKeyDown(KeyCode.Space) && movement.magnitude > 0.01f && dodgeTimer == 0f && stamina >= dodgeStaminaCost)
        {
            dodgeTimer += dodgeTime;
            //transform.Find("Cube").GetComponent<SkinnedMeshRenderer>().enabled = false;
            transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
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
            transform.localScale = new Vector3(1f, 1f, 1f);
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
                equippedWeaponObj = Instantiate(weaponEquipmentArr[selectedWeaponIndex - 1], rightHandBoneRef.transform);

                WeaponController weaponController = equippedWeaponObj.GetComponent<WeaponController>();

                weaponController.ownerObjRef = gameObject;
                // вращение и движение оружия так, чтобы оно было в правой руке (подходит для пистолета)
                equippedWeaponObj.transform.localPosition = weaponController.rightHandPostionForWeapon;
                equippedWeaponObj.transform.localRotation = Quaternion.Euler(weaponController.rightHandRotationForWeapon);
                equippedWeaponObj.transform.localScale = weaponController.rightHandScaleForWeapon;
            }
        }

        // убрать экипированное оружие
        if (selectedWeaponIndex == 0)
        {
            Destroy(equippedWeaponObj);
        }

        // стрельба с предварительным прицеливанием
        if (Input.GetMouseButton(1) && equippedWeaponObj != null)
        {
            Vector3 pos = equippedWeaponObj.transform.Find("BulletOutPoint").gameObject.transform.position;
            Vector3 dir = mousePosOnGround + new Vector3(0.0f, 1.5f, 0.0f);
            Debug.DrawLine(pos, dir, Color.yellow);

            // отрисовка прицела
            aimSpotRef.gameObject.SetActive(true);
            aimSpotRef.transform.LookAt(cam.transform.position);
            //aimSpotRef.rectTransform.position  = new Vector3(mousePosOnGround.x, mousePosOnGround.z, 0f);
            //Debug.Log(aimSpotImageRef.raycastTarget);

            isAiming = true;
            if (Input.GetMouseButton(0))
            {
                FindInAllChildren(transform, "RightHandController", ref rightHandBoneController);
                equippedWeaponObj.GetComponent<WeaponController>().Shoot(0, rightHandBoneController.transform.position);
            }
        }
        // стрельба без прицеливания
        if (Input.GetMouseButton(0) && /* !isAiming &&*/  equippedWeaponObj != null)
        {
            FindInAllChildren(transform, "RightHandController", ref rightHandBoneController);
            equippedWeaponObj.GetComponent<WeaponController>().Shoot(0, rightHandBoneController.transform.position);
        }

        // перезарядка
        if (Input.GetKeyDown(KeyCode.R) && equippedWeaponObj != null)
        {
            equippedWeaponObj.GetComponent<WeaponController>().Reload();
        }

        // Отмена прицеливания когда опускаем пкм
        if (Input.GetMouseButtonUp(1))
        {

            isAiming = alwaysAiming;
            aimSpotRef.gameObject.SetActive(false);
        }

        
    }
}
