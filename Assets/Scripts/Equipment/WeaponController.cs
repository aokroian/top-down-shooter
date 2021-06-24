using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;

/*
public enum ShootingModes
{
    Automatic,
    Single,
}
*/


public class WeaponController : MonoBehaviour, IAmmoConsumer
{
    // variables for animation
    private Rig weaponAimRigLayer;
    private Animator animator;
    private bool isReloading = false;


    //
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
    //public ShootingModes shootingMode = ShootingModes.Single;


    public Vector3 localPosition;
    public Vector3 localRotation;
    public Vector3 localScale;

    public GameObject bulletFlashRef;
    public GameObject bulletOutPointObj;

    public GameObject hitEffectRef;

    public GameObject bulletObjRef;
    public float bulletVelocity = 30f;
    public int bulletsInClip = 0;

    public float maxShotAngle = 10f;

    public AmmoType ammoType;

    [HideInInspector]
    public IAmmoProvider ammoProvider;

    private GameObject aimAtPoint;
    public float reloadTimer { get; private set; } = 0f;
    private float nextShotTimer = 0f;

    private void Start()
    {
        //get weapon animator
        animator = gameObject.GetComponent<Animator>();
        // get owners weapon aim rig layer (needed to disable it playing animations)
        weaponAimRigLayer = ownerObjRef.transform.Find("RigLayer_WeaponAim").gameObject.GetComponent<Rig>();

        // установка дефолтных значений таймеров и патронов в обойме
        bulletsInClip = clipSize;
        reloadTimer = 0f;
        nextShotTimer = 0f;
    }

    private void Update()
    {
        // animation part
        if (animator != null)
        {
            
            if (isReloading)
            {
                // disable weapon aim
                if (weaponAimRigLayer != null)
                {
                    weaponAimRigLayer.weight = 0f;
                }
                //disable laser aim
                gameObject.GetComponent<LaserAim>().isEnabled = false;

                // start reloading animation
                animator.SetBool("is reloading", true);
                
            }
            else
            {
                if (weaponAimRigLayer != null)
                {
                    weaponAimRigLayer.weight = 1f;
                }
                animator.SetBool("is reloading", false);
                gameObject.GetComponent<LaserAim>().isEnabled = true;
            }
        }
        


        // таймеры
        if (reloadTimer > 0f)
        {
            isReloading = true;
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
        if (reloadTimer == 0f)
        {
            isReloading = false;
        }
        if (nextShotTimer < 0f)
        {
            nextShotTimer = 0f;
        }

        // если патронов в магазине не осталось, происходит автоматическая перезарядка
        if (bulletsInClip < 1 && ammoProvider.HasAmmo(ammoType))
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

        ShotRecoil();

        if (bulletsInClip >= 1)
        {
            //Debug.DrawLine(pos, targetPos, Color.green, Mathf.Infinity);
            // частицы у ствола
            if (bulletFlashRef != null)
                Instantiate(bulletFlashRef, bulletOutPointObj.transform.position, bulletOutPointObj.transform.rotation);

            // убираем пулю из обоймы
            bulletsInClip -= 1;

            GameObject instantiatedProjectile = Instantiate(bulletObjRef, bulletOutPointObj.transform.position, bulletOutPointObj.transform.rotation);

            var bulletController = instantiatedProjectile.GetComponent<BulletController>();

            // данные об уроне, силе толчка и эффекте на поверхности, куда попала пуля
            bulletController.bulletImpactForce = shotImpactForce;
            bulletController.shotDamage = shotDamage + shotDamageModifier;
            bulletController.hitEffectRef = hitEffectRef;
            bulletController.shooter = ownerObjRef;

            // скорость и направление полета пули
            Vector3 shotDir = targetPos - bulletOutPointObj.transform.position;
            shotDir.y = 0.0f;
            shotDir = shotDir.normalized * bulletVelocity;
            shotDir = GetScatterAngle() * shotDir;
            bulletController.SetVelocity(shotDir);
            instantiatedProjectile.transform.rotation = Quaternion.LookRotation(Vector3.up, shotDir);
        }
        // таймер времени на выстрел
        nextShotTimer = rateOfFire;
    }

    public Quaternion GetScatterAngle()
    {
        float halfMaxAngle = maxShotAngle / 2;
        float angle1 = Random.Range(0 - halfMaxAngle, halfMaxAngle);
        float angle2 = Random.Range(0 - halfMaxAngle, halfMaxAngle);
        var result = Quaternion.Euler(0f, Mathf.Abs(angle1) < Mathf.Abs(angle2) ? angle1 : angle2, 0f);
        return result;
    }

    public void ShotRecoil()
    {
        FindInAllChildren(ownerObjRef.transform, "AimAtPoint", ref aimAtPoint);
        if (aimAtPoint != null)
        {
            // направление отдачи
            Vector3 offset = new Vector3(0f, recoilHeight, 0f);
            aimAtPoint.transform.position = aimAtPoint.transform.position + offset;
        }
    }


    public void Reload()
    {
        // невозможно перезарядиться, если уже идет перезарядка
        if (reloadTimer > 0f) return;
        // устанавливаем таймер
        reloadTimer = reloadTime;

        int needToAdd = clipSize - bulletsInClip;

        // когда патроны остались только в магазине, перезарядка невозможна
        if (!ammoProvider.HasAmmo(ammoType)) return;
        
        // TODO: Should be at end of reloading?
        bulletsInClip += ammoProvider.GetAmmo(ammoType, needToAdd);
    }

    public int GetAmmoLeft()
    {
        return bulletsInClip;
    }

    public AmmoType GetAmmoType()
    {
        return ammoType;
    }
}
