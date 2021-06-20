using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

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

    // constraint for animation rigging (weapon aim part)
    private GameObject weaponAimConstraintObj;

    private GameObject equippedWeaponObj;

    private Transform player;

    private NavMeshAgent agent;

    private State currentState = State.IDLE;

    private float shootTimer;
    private float shootRange;


    private Vector3 prevFramePosition;
    private float prevFramePosDetectionTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        shootRange = agent.stoppingDistance + additionalShootRange;

        // spawn weapon
        equippedWeaponObj = Instantiate(weapon, parentBoneForWeapon.transform);

        WeaponController weaponController = equippedWeaponObj.GetComponent<WeaponController>();

        weaponController.ownerObjRef = gameObject;
        // moving the weapon to the desired position
        equippedWeaponObj.transform.localPosition = weaponController.localPosition;
        equippedWeaponObj.transform.localRotation = Quaternion.Euler(weaponController.localRotation);
        equippedWeaponObj.transform.localScale = weaponController.localScale;

        // animation rigging variables
        FindInAllChildren(gameObject.transform, "RightHandController", ref rightHandConstraintController);
        FindInAllChildren(gameObject.transform, "LeftHandController", ref leftHandConstraintController);
        FindInAllChildren(gameObject.transform, "AimWeapon", ref weaponAimConstraintObj);
        FindInAllChildren(gameObject.transform, "RigLayer_HandsPosition", ref rigLayerHandsPosition);

        FindInAllChildren(equippedWeaponObj.transform, "RightHandPoint", ref rightHandPoint);
        FindInAllChildren(equippedWeaponObj.transform, "LeftHandPoint", ref leftHandPoint);

        weaponAimConstraintObj.GetComponent<MultiAimConstraint>().data.constrainedObject = equippedWeaponObj.transform;

        prevFramePosition = transform.position;
        prevFramePosDetectionTimer = posDetectionDelay;
    }

    // Update is called once per frame
    void Update()
    {
        // hand on weapon animation rigging part
        // moving hand rig controllers to points on weapon when its equiped
        

        rightHandConstraintController.transform.position = rightHandPoint.transform.position;
        leftHandConstraintController.transform.position = leftHandPoint.transform.position;


        // rotating enemy towards player
        Vector3 lTargetDir = aimSpotRef.transform.position - transform.position;
        lTargetDir.y = 0.0f;
        gameObject.transform.rotation = Quaternion.LookRotation(lTargetDir);



        // decrementing timer which is needed to get next difference in position
        prevFramePosDetectionTimer -= Time.deltaTime;
        if (prevFramePosDetectionTimer < 0f)
            prevFramePosDetectionTimer = 0f;

        // setting floats for animation controller
        Vector3 currentFramePos = transform.position;
        Vector3 posDifference = currentFramePos - prevFramePosition;
        if (prevFramePosDetectionTimer == 0f)
        {
            
            Vector3 clampedVelocity = Vector3.ClampMagnitude(posDifference, 1f);
            Vector3 globalMovement = new Vector3(clampedVelocity.x, 0.0f, clampedVelocity.y);
            Vector3 localMovement = gameObject.transform.InverseTransformDirection(globalMovement);
            gameObject.GetComponent<Animator>().SetFloat("local Z speed", localMovement.z);
            gameObject.GetComponent<Animator>().SetFloat("local X speed", localMovement.x);

            prevFramePosition = currentFramePos;
            prevFramePosDetectionTimer = posDetectionDelay;
        }
        


        if (posDifference.x <= minPosChangeForAnimation && posDifference.x <= minPosChangeForAnimation)
            gameObject.GetComponent<Animator>().SetBool("Is Idle", true);
        else
            gameObject.GetComponent<Animator>().SetBool("Is Idle", false);
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
                    equippedWeaponObj.GetComponent<WeaponController>().Shoot(0, aimSpotRef.transform.position);
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
        if (aimSpotRef != null)
            aimSpotRef.transform.position = player.position;
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

    public void OnEnemyDies()
    {
        diesEvent.Raise(new EnemyEventParam(transform.position, cost));
    }
}
