using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootingEnemyController : MonoBehaviour
{
    public int amountOfBullets;
    public GameObject weapon;

    private GameObject rightHandBoneRef;
    private GameObject rightHandRigControllerObj;
    private GameObject equippedWeaponObj;

    public int cost;

    public bool playerAwared;
    public float visionRange = 15.0f;

    private Transform player;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;

        // spawn weapon
        FindInAllChildren(gameObject.transform, "hand.R", ref rightHandBoneRef);
        FindInAllChildren(gameObject.transform, "RightHandController", ref rightHandRigControllerObj);
        if (rightHandBoneRef != null && rightHandRigControllerObj != null)
        {
            equippedWeaponObj = Instantiate(weapon, rightHandBoneRef.transform);
            WeaponController weaponController = equippedWeaponObj.GetComponent<WeaponController>();

            weaponController.ownerObjRef = gameObject;
            // correcting weapon obj transform for right hand
            equippedWeaponObj.transform.localPosition = weaponController.rightHandPostionForWeapon;
            equippedWeaponObj.transform.localRotation = Quaternion.Euler(weaponController.rightHandRotationForWeapon);
            equippedWeaponObj.transform.localScale = weaponController.rightHandScaleForWeapon;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAwared)
        {
            agent.destination = player.position;

            // always aim at user
            if (rightHandRigControllerObj != null)
                rightHandRigControllerObj.transform.position = player.position;
                equippedWeaponObj.GetComponent<WeaponController>().Shoot(0, rightHandRigControllerObj.transform.position);
        }
        else
        {
            playerAwared = IsPlayerSpotted();
        }
    }

    private bool IsPlayerSpotted()
    {
        var enemyPos = new Vector3(transform.position.x, 1.0f, transform.position.z);
        var playerPos = new Vector3(player.transform.position.x, 1.0f, player.transform.position.z);
        bool result = false;
        if (Vector3.Distance(enemyPos, playerPos) <= visionRange)
        {
            result = !Physics.Linecast(enemyPos, playerPos, LayerMask.GetMask("Obstacle"));
        }

        return result;
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
}
