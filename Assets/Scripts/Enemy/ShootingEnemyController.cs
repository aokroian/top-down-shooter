using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootingEnemyController : MonoBehaviour, EnemyProperties
{
    enum State
    {
        IDLE,
        CHASING,
        BEFORE_SHOT,
        AFTER_SHOT,
        MOVING
    }

    [SerializeField]
    private int _cost;
    public int cost { get => _cost; set => _cost = value; }

    public int amountOfBullets;
    public GameObject weapon;
    public bool playerAwared;
    public float visionRange = 15.0f;

    public float shootTime = 1.0f;
    public float walkDistanceMin = 1.0f;
    public float walkDistanceMax = 2.0f;
    public float additionalShootRange = 2.0f;

    private GameObject rightHandBoneRef;
    private GameObject rightHandRigControllerObj;
    private GameObject equippedWeaponObj;

    private Transform player;

    private NavMeshAgent agent;

    private State currentState = State.IDLE;

    private float shootTimer;
    private float shootRange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        shootRange = agent.stoppingDistance + additionalShootRange;

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
        /*
        if (playerAwared)
        {
            // always aim at user
            if (rightHandRigControllerObj != null)
                rightHandRigControllerObj.transform.position = player.position;
                //equippedWeaponObj.GetComponent<WeaponController>().Shoot(0, rightHandRigControllerObj.transform.position);
            Act();
        }
        else
        {
            playerAwared = IsPlayerSpotted();
        }
        */
        Act();
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

    private void Act()
    {
        switch (currentState)
        {
            case State.IDLE:
                if (IsPlayerSpotted())
                {
                    playerAwared = true;
                    currentState = State.CHASING;
                    //Debug.Log("state: " + currentState);
                }
                break;
            case State.CHASING:
                agent.isStopped = false;
                if (IsAtDestination())
                {
                    currentState = State.BEFORE_SHOT;
                    shootTimer = shootTime;
                    //Debug.Log("state: " + currentState);
                }
                else
                {
                    agent.destination = player.position;
                    AimPlayer();
                }
                break;
            case State.BEFORE_SHOT:
                // Check if player in shoot range?
                agent.isStopped = true;
                AimPlayer();
                shootTimer -= Time.deltaTime;
                if (shootTimer <= shootTime / 2f)
                {
                    equippedWeaponObj.GetComponent<WeaponController>().Shoot(0, rightHandRigControllerObj.transform.position);
                    currentState = State.AFTER_SHOT;
                    //Debug.Log("state: " + currentState);
                }
                break;
            case State.AFTER_SHOT:
                shootTimer -= Time.deltaTime;
                if (shootTimer <= 0f)
                {
                    agent.destination = GetRandomMovementPoint();
                    currentState = State.MOVING;
                    //Debug.Log("state: " + currentState);
                }
                break;
            case State.MOVING:
                agent.isStopped = false;
                if (Vector3.Distance(transform.position, player.transform.position) > shootRange)
                {
                    agent.destination = player.transform.position;
                    currentState = State.CHASING;
                    //Debug.Log("state: " + currentState);
                }
                else if (IsAtDestination())
                {
                    currentState = State.BEFORE_SHOT;
                    shootTimer = shootTime;
                    //Debug.Log("state: " + currentState);
                }
                break;
            default:
                break;
        }
    }

    private void AimPlayer()
    {
        if (rightHandRigControllerObj != null)
            rightHandRigControllerObj.transform.position = player.position;
    }

    private Vector3 GetRandomMovementPoint()
    {
        float deltaAngle = Random.Range(-45f, 45f);
        float directionAngle = Random.value > 0.5f ? 90f : -90f;
        Quaternion rotation = Quaternion.Euler(0f, directionAngle + deltaAngle, 0f);
        Vector3 startDirection = Vector3.Normalize(transform.position - player.transform.position);
        return (rotation * startDirection) * (Random.Range(walkDistanceMin, walkDistanceMax) + agent.stoppingDistance) + transform.position;
    }

    private bool IsAtDestination()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    }
}
