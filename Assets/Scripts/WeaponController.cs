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
    public float ownerAgility = 1f;
    public float ownerRigControllerAgility = 1f;
    public GameObject ownerObjRef;


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

    public bool shootWithRaycast = true;
    public GameObject muzzleObjRef;
    public float muzzleVelocity;
    public int bulletsInClip = 0;

    private PlayerController playerController;
    private ShootingEnemyController enemyController;

    private GameObject rightHandBoneController;
    private float reloadTimer = 0f;
    private float nextShotTimer = 0f;

    private GameObject mainUIRef;
    private int amountOfBullets;
    private void Start()
    {
        playerController = ownerObjRef.GetComponent<PlayerController>();
        enemyController = ownerObjRef.GetComponent<ShootingEnemyController>();

        

        // установка дефолтных значений таймеров и патронов в обойме
        bulletsInClip = clipSize;
        reloadTimer = 0f;
        nextShotTimer = 0f;

        // поиск объекта UI
        mainUIRef = GameObject.FindGameObjectWithTag("MainUI");


        if (playerController != null)
        {
            amountOfBullets = playerController.amountOfBullets;
        } else if (enemyController != null)
        {
            amountOfBullets = enemyController.amountOfBullets;
        }
        
    }

    private void Update()
    { 

        // таймеры
        if (reloadTimer > 0f)
        {
            reloadTimer -= Time.deltaTime;
            //playerRef.GetComponent<PlayerController>().isAiming = playerRef.GetComponent<PlayerController>().alwaysAiming;
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

        // changes in user interface
        if (ownerObjRef.name == "Player")
        {
            mainUIRef.gameObject.transform.Find("BulletsInClipText").gameObject.GetComponent<Text>().text = bulletsInClip.ToString();
            mainUIRef.gameObject.transform.Find("AmountOfBulletsText").gameObject.GetComponent<Text>().text = amountOfBullets.ToString();
            mainUIRef.gameObject.transform.Find("ReloadScrollbar").gameObject.GetComponent<Scrollbar>().value = reloadTimer / reloadTime;
            mainUIRef.gameObject.transform.Find("ReloadScrollbar").gameObject.GetComponent<Image>().color = Color.Lerp(Color.blue, Color.red, reloadTimer / reloadTime);
        }
        

        // если патронов в магазине не осталось, происходит автоматическая перезарядка
        if (bulletsInClip < 1 && amountOfBullets > 0)
        {
            Reload();
        }
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
        // позиция невидимого объекта, который нужно разместить в том месте ствола, откуда будет производиться выстрел
        Vector3 originPointOfShot = transform.TransformPoint(transform.Find("BulletOutPoint").gameObject.transform.localPosition);
        // направление от места вылета пули к цели
        Vector3 direction = (targetPos - transform.position);

        // отдача
        FindInAllChildren(ownerObjRef.transform, "RightHandController", ref rightHandBoneController);
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
            if (muzzleFlashRef != null)
                Instantiate(muzzleFlashRef, bulletOutPointObj.transform.position, bulletOutPointObj.transform.rotation);
            
            // убираем пулю из обоймы
            bulletsInClip -= 1;

            if (!shootWithRaycast)
            {
                Vector3 muzzleSpawnPos = new Vector3 (bulletOutPointObj.transform.position.x, bulletOutPointObj.transform.position.y, bulletOutPointObj.transform.position.z + 2f);
                GameObject instantiatedProjectile = Instantiate(muzzleObjRef, bulletOutPointObj.transform.TransformPoint(muzzleSpawnPos), bulletOutPointObj.transform.rotation);

                // данные об уроне, силе толчка и эффекте на поверхности, куда попала пуля
                instantiatedProjectile.GetComponent<MuzzleController>().bulletImpactForce = shotImpactForce;
                instantiatedProjectile.GetComponent<MuzzleController>().shotDamage = shotDamage + shotDamageModifier;
                instantiatedProjectile.GetComponent<MuzzleController>().hitEffectRef = hitEffectRef;

                Vector3 shotDir = targetPos - bulletOutPointObj.transform.position;
                shotDir.y = 0.0f;
                // скорость и направление полета пули
                instantiatedProjectile.GetComponent<Rigidbody>().velocity = shotDir.normalized * muzzleVelocity;
                instantiatedProjectile.transform.rotation = Quaternion.LookRotation(Vector3.up, instantiatedProjectile.GetComponent<Rigidbody>().velocity);
            } else
            {
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

            if (playerController != null)
            {
                playerController.amountOfBullets -= bulletsInClip;
            }
            if (enemyController != null)
            {
                enemyController.amountOfBullets -= bulletsInClip;
            }
            

        }
        // если происходит перезарядка полностью израсходованного магазина
        else
        {
            // когда общее кол-во патронов больше размера магазина
            if (amountOfBullets >= needToAdd)
            {
                bulletsInClip += needToAdd;

                if (playerController != null)
                {
                    playerController.amountOfBullets -= needToAdd;
                }
                if (enemyController != null)
                {
                    enemyController.amountOfBullets -= needToAdd;
                }
            }
            // когда общее кол-во патронов меньше размера магазина
            else
            {
                bulletsInClip += amountOfBullets;

                if (playerController != null)
                {
                    playerController.amountOfBullets = 0;
                }
                if (enemyController != null)
                {
                    enemyController.amountOfBullets = 0;
                }
            }

            
            
        }


        if (playerController != null)
        {
            amountOfBullets = playerController.amountOfBullets;
        }
        if (enemyController != null)
        {
            amountOfBullets = enemyController.amountOfBullets;
        }

        
    }
}
