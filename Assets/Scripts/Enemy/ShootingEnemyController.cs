using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class ShootingEnemyController : MonoBehaviour, EnemyProperties
{
    public enum State
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
    public State defaultState = State.IDLE;
    public float visionRange = 15.0f;

    public float shootTime = 1.0f;
    public float walkDistanceMin = 1.0f;
    public float walkDistanceMax = 2.0f;
    public float shootRange = 8.0f;

    // animation rigging variables
    public GameObject rightHandConstraintController;
    public GameObject leftHandConstraintController;
    public GameObject rigLayerHandsPosition;
    public GameObject parentBoneForWeapon;
    public GameObject aimSpotRef;

    private GameObject rightHandPoint;
    private GameObject leftHandPoint;

    //
    public float posDetectionDelay = 0.1f;
    public float minPosChangeForAnimation = 0.1f;

    public EnemyDiesEvent diesEvent;

    public float stopPursuitDistance = 20f;

    // constraint for animation rigging (weapon aim part)
    private GameObject weaponAimConstraintObj;

    private GameObject equippedWeaponObj;

    private Transform player;

    private NavMeshAgent agent;

    private State currentState = State.IDLE;

    private float shootTimer;

    // Maybe make scriptable object? Now every enemy create additional object
    private IAmmoProvider ammoProvider = new EnemyEndlessAmmoProvider();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;

        currentState = defaultState;

        // spawn weapon
        equippedWeaponObj = Instantiate(weapon, parentBoneForWeapon.transform);

        WeaponController weaponController = equippedWeaponObj.GetComponent<WeaponController>();

        weaponController.ownerObjRef = gameObject;
        weaponController.ammoProvider = ammoProvider;
        // moving the weapon to the desired position
        equippedWeaponObj.transform.localPosition = weaponController.localPosition;
        equippedWeaponObj.transform.localRotation = Quaternion.Euler(weaponController.localRotation);
        equippedWeaponObj.transform.localScale = weaponController.localScale;

        // animation rigging variables
        //FindInAllChildren(gameObject.transform, "RightHandController", ref rightHandConstraintController);
        //FindInAllChildren(gameObject.transform, "LeftHandController", ref leftHandConstraintController);
        //FindInAllChildren(gameObject.transform, "AimWeapon", ref weaponAimConstraintObj);
        //FindInAllChildren(gameObject.transform, "RigLayer_HandsPosition", ref rigLayerHandsPosition);

        //FindInAllChildren(equippedWeaponObj.transform, "RightHandPoint", ref rightHandPoint);
        //FindInAllChildren(equippedWeaponObj.transform, "LeftHandPoint", ref leftHandPoint);

        //weaponAimConstraintObj.GetComponent<MultiAimConstraint>().data.constrainedObject = equippedWeaponObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // hand on weapon animation rigging part
        // moving hand rig controllers to points on weapon when its equiped
        

        //rightHandConstraintController.transform.position = rightHandPoint.transform.position;
        //leftHandConstraintController.transform.position = leftHandPoint.transform.position;


        // rotating enemy towards player
        Vector3 lTargetDir = aimSpotRef.transform.position - transform.position;
        if (lTargetDir.x != 0f && lTargetDir.z != 0f)
        {
            lTargetDir.y = 0.0f;
            gameObject.transform.rotation = Quaternion.LookRotation(lTargetDir);
        }

        Vector3 velocity = agent.velocity;
        if (velocity.magnitude <= 0.1f)
        {
            gameObject.GetComponent<Animator>().SetBool("Is Idle", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("Is Idle", false);
        }

        Vector3 globalMovement = new Vector3(velocity.x, 0.0f, velocity.z);
        Vector3 localMovement = gameObject.transform.InverseTransformDirection(globalMovement);
        gameObject.GetComponent<Animator>().SetFloat("local Z speed", localMovement.z);
        gameObject.GetComponent<Animator>().SetFloat("local X speed", localMovement.x);

        // meele animation
        Vector3 distanceToPlayer = gameObject.transform.position - player.transform.position;
        //gameObject.GetComponent<Animator>().SetFloat("distance to player", distanceToPlayer.magnitude / 2);
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
        bool result = false;
        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange)
        {
            result = IsNoObstacle();
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
                    currentState = State.CHASING;
                    //Debug.Log("state: " + currentState);
                }
                break;
            case State.CHASING:
                if (Vector3.Distance(transform.position, player.transform.position) > stopPursuitDistance)
                {
                    currentState = State.IDLE;
                    agent.destination = transform.position;
                    agent.isStopped = true;
                }
                agent.isStopped = false;
                agent.destination = player.position;
                AimPlayer();
                if (IsAtShootingRange() && IsNoObstacle())
                {
                    currentState = State.BEFORE_SHOT;
                    shootTimer = shootTime;
                    //Debug.Log("state: " + currentState);
                }
                break;
            case State.BEFORE_SHOT:
                // Check if player in shoot range?
                agent.isStopped = true;
                AimPlayer();
                shootTimer -= Time.deltaTime;
                if (shootTimer <= shootTime / 2f)
                {
                    equippedWeaponObj.GetComponent<WeaponController>().Shoot(0);
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
                if (!IsAtShootingRange() || !IsNoObstacle())
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
        if (aimSpotRef != null)
            aimSpotRef.transform.position = player.position;
    }

    private Vector3 GetRandomMovementPoint()
    {
        float deltaAngle = Random.Range(-45f, 45f);
        float directionAngle = Random.value > 0.5f ? 90f : -90f;
        Quaternion rotation = Quaternion.Euler(0f, directionAngle + deltaAngle, 0f);
        Vector3 startDirection = Vector3.Normalize(transform.position - player.transform.position);
        return (rotation * startDirection) * Random.Range(walkDistanceMin, walkDistanceMax) + transform.position;
    }

    private bool IsAtDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    private bool IsNoObstacle()
    {
        var enemyPos = new Vector3(transform.position.x, 1.0f, transform.position.z);
        var playerPos = new Vector3(player.transform.position.x, 1.0f, player.transform.position.z);
        return !Physics.Linecast(enemyPos, playerPos, LayerMask.GetMask("Obstacle"));
    }

    private bool IsAtShootingRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= shootRange;
    }

    public void OnEnemyDies()
    {
        diesEvent.Raise(new EnemyEventParam(transform.position, cost));
    }

    public void Aware(AwareEventParam param)
    {
        if (currentState == State.IDLE && Vector3.Distance(param.position, transform.position) <= param.distance)
        {
            currentState = State.CHASING;
        }
    }
}
