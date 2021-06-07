using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ShootingModes
{
    Automatic,
    Single,
}


public class WeaponController : MonoBehaviour
{
    public float playerRotationSpeed = 1f;
    public float rigMovementSpeed = 1f;


    public float shootRange = 100f;
    public float shotDamage = 0f;
    public float shotImpactForce = 100f;
    public float recoilHeight = 1f;
    public int clipSize = 5;
    public float rateOfFire = 0f;
    public float reloadTime = 0f;
    public ShootingModes shootingMode = ShootingModes.Single;


    public Vector3 rightHandPostionForWeapon;
    public Vector3 rightHandRotationForWeapon;
    public Vector3 rightHandScaleForWeapon;

    public GameObject muzzleFlashRef;
    public GameObject bulletOutPointObj;

    public GameObject hitEffectRef;

    private GameObject playerRef;
    private GameObject handRigObjectRef;
    private float playerDefaultRotationSpeed;
    private float defaultRigMovementSpeed;

    public int bulletsInClip = 0;
    private GameObject rightHandBoneController;

    private float reloadTimer = 0f;
    private float nextShotTimer = 0f;

    private GameObject mainUIRef;
    private int amountOfBullets;
    private void Start()
    {
        

        // установка дефолтных значений таймеров и патронов в обойме
        bulletsInClip = clipSize;
        reloadTimer = 0f;
        nextShotTimer = 0f;

        // поиск объекта игрока и отдельно контроллера рига руки для последующей модификации их параметров
        playerRef = GameObject.FindGameObjectWithTag("Player");
        handRigObjectRef = GameObject.FindGameObjectWithTag("RightHandConstraint");

        // запись дефолтных параметров игрока для последующего возврата к ним после снятия данного оружия
        playerDefaultRotationSpeed = playerRef.GetComponent<PlayerController>().rotationSpeed;
        defaultRigMovementSpeed = handRigObjectRef.GetComponent<RigController>().movementRapidity;

        // поиск объекта UI
        mainUIRef = GameObject.FindGameObjectWithTag("MainUI");


        amountOfBullets = playerRef.GetComponent<PlayerController>().amountOfBullets;
    }

    private void Update()
    {
        // влияние на скорость поворота корпуса игрока и скорость движения руки с оружием 
        if (playerDefaultRotationSpeed > 0f && defaultRigMovementSpeed > 0f)
        {
            playerRef.GetComponent<PlayerController>().rotationSpeed = playerRotationSpeed;
            handRigObjectRef.GetComponent<RigController>().movementRapidity = rigMovementSpeed;
        }


        // таймеры
        if (reloadTimer > 0f)
        {
            reloadTimer -= Time.deltaTime;
            playerRef.GetComponent<PlayerController>().isAiming = false;
        }
        if (nextShotTimer > 0f)
        {
            nextShotTimer -= Time.deltaTime;
        }
        if (reloadTimer < 0f)
        {
            reloadTimer = 0f;
            //playerRef.GetComponent<PlayerController>().isAiming = false;
        }
        if (nextShotTimer < 0f)
        {
            nextShotTimer = 0f;
        }

        // обновление ui
        mainUIRef.gameObject.transform.Find("BulletsInClipText").gameObject.GetComponent<Text>().text = "BulletsInClip: " + bulletsInClip.ToString();
        mainUIRef.gameObject.transform.Find("AmountOfBulletsText").gameObject.GetComponent<Text>().text = "AmountOfBullets: " + amountOfBullets.ToString();
        mainUIRef.gameObject.transform.Find("ReloadTimerText").gameObject.GetComponent<Text>().text = "ReloadTimer: " + Mathf.RoundToInt(reloadTimer).ToString();

        if (reloadTimer > 0f)
        {
            mainUIRef.gameObject.transform.Find("ReloadTimerText").gameObject.GetComponent<Text>().color = Color.red;
        } else
        {
            mainUIRef.gameObject.transform.Find("ReloadTimerText").gameObject.GetComponent<Text>().color = Color.black;
        }

        // если патронов в магазине не осталось, происходит автоматическая перезарядка
        if (bulletsInClip < 1 && amountOfBullets > 0)
        {
            Reload();
        }
     }
    private void OnDestroy()
    {
        // когда игрок перестает использовать оружие возвращаем стандартные параметры 
        playerRef.GetComponent<PlayerController>().rotationSpeed = playerDefaultRotationSpeed;
        handRigObjectRef.GetComponent<RigController>().movementRapidity = defaultRigMovementSpeed;
    }


    // вспомогательный метод для поиска вложенного объекта по имени с проходом по всем вложенным объектам
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


    public void Shoot(float shotDamageModifier, Vector3 targetPos)
    {
        // если один из таймеров еще не достиг нуля, выстрел невозможен
        if (reloadTimer > 0f || nextShotTimer > 0f) return;
        // если в обойме нет патронов, вместо выстрела производится перезарядка
        if (bulletsInClip <= 0)
        {
            Reload();
            return;
        }

        playerRef.GetComponent<PlayerController>().isAiming = true;
        // позиция невидимого объекта, который нужно разместить в том месте ствола, откуда будет производиться выстрел
        Vector3 originPointOfShot = transform.TransformPoint(transform.Find("BulletOutPoint").gameObject.transform.localPosition);
        // направление от места вылета пули к цели
        Vector3 direction = (targetPos - transform.position);

        // отдача
        FindInAllChildren(playerRef.transform, "RightHandController", ref rightHandBoneController);
        if (rightHandBoneController != null)
        {
            // направление отдачи
            Vector3 offset = new Vector3(0f, recoilHeight, 0f);
            rightHandBoneController.transform.position = rightHandBoneController.transform.position + offset;
        }


        if (bulletsInClip >= 1)
        {
            //Debug.DrawLine(pos, targetPos, Color.green, Mathf.Infinity);


            // частицы у ствола 
            Instantiate(muzzleFlashRef, bulletOutPointObj.transform.position, bulletOutPointObj.transform.rotation);

            // убираем пулю из обоймы
            bulletsInClip -= 1;

            // если луч попал во что-то
            RaycastHit hit;
            if (Physics.Raycast(originPointOfShot, direction, out hit, shootRange))
            {
                // частицы на цели (потом нужно убрать логику в саму цель, чтобы были разные)
                Instantiate(hitEffectRef, hit.point, Quaternion.LookRotation(hit.normal));



                // толчок пулей на объект попадания
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(-hit.normal * shotImpactForce, hit.point);
                }


                // урон объекту
                Target target = hit.transform.GetComponent<Target>();
                if (target != null)
                {
                    target.TakeDamage(shotDamage + shotDamageModifier);
                }
            }

        }

        // таймер времени на выстрел
        nextShotTimer = rateOfFire;

    }


    public void Reload()
    {
        // невозможно перезарядиться, если уже идет перезарядка
        if (reloadTimer > 0f) return;
        // устанавливаем таймер
        reloadTimer = reloadTime;

        int needToAdd = clipSize - bulletsInClip;
        


        // когда патроны остались только в магазине, перезарядка невозможна
        if (amountOfBullets <= 0f) return;


        // если происходит перезарядка полностью израсходованного магазина
        if (needToAdd == clipSize)
        {
            // когда общее кол-во патронов больше размера магазина
            if (amountOfBullets >= clipSize)
            {
                bulletsInClip = clipSize;
            }
            // когда общее кол-во патронов меньше размера магазина
            else
            {
                bulletsInClip = amountOfBullets;
            }

            playerRef.GetComponent<PlayerController>().amountOfBullets -= bulletsInClip;

        }
        // если происходит перезарядка полностью израсходованного магазина
        else
        {
            // когда общее кол-во патронов больше размера магазина
            if (amountOfBullets >= needToAdd)
            {
                bulletsInClip += needToAdd;
                playerRef.GetComponent<PlayerController>().amountOfBullets -= needToAdd;
            }
            // когда общее кол-во патронов меньше размера магазина
            else
            {
                bulletsInClip += amountOfBullets;
                playerRef.GetComponent<PlayerController>().amountOfBullets = 0;
            }

            
            
        }




        amountOfBullets = playerRef.GetComponent<PlayerController>().amountOfBullets;
    }
}
