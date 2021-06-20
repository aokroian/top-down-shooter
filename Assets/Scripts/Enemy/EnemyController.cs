using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, EnemyProperties
{
    enum State
    {
        IDLE,
        CHASING,
        BEFORE_BITE,
        AFTER_BITE
    }

    [SerializeField]
    private int _cost;
    public int cost { get => _cost; set => _cost = value; }

    public bool playerAwared;
    public float visionRange = 15.0f;

    public float damage = 20f;
    public float biteDistance = 1.0f;
    public float beforeBiteTime = 0.2f;
    public float afterBiteTime = 0.8f;

    public float posDetectionDelay = 0.1f;
    public float minPosChangeForAnimation = 0.1f;
    public float biteTimer;

    public EnemyDiesEvent diesEvent;

    private State currentState;
    private Transform player;

    private NavMeshAgent agent;

    // variables to detect position change
    private Vector3 prevFramePosition;
    private float prevFramePosDetectionTimer;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;

        prevFramePosition = transform.position;
        prevFramePosDetectionTimer = posDetectionDelay;
    }

    // Update is called once per frame
    void Update()
    {
        // rotate towards player
        Vector3 lTargetDir = player.transform.position - transform.position;
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

        // meele animation
        Vector3 distanceToPlayer = gameObject.transform.position - player.transform.position;
        gameObject.GetComponent<Animator>().SetFloat("distance to player", distanceToPlayer.magnitude/2);


        if (posDifference.x <= minPosChangeForAnimation && posDifference.x <= minPosChangeForAnimation)
            gameObject.GetComponent<Animator>().SetBool("Is Idle", true);
        else
            gameObject.GetComponent<Animator>().SetBool("Is Idle", false);
        /*
        if (playerAwared) {
            agent.destination = player.position;
        } else
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

    private void Act()
    {
        switch (currentState)
        {
            case State.IDLE:
                if (IsPlayerSpotted())
                {
                    playerAwared = true;
                    currentState = State.CHASING;
                    agent.destination = player.position;
                    //Debug.Log("state: " + currentState);
                }
                break;
            case State.CHASING:
                agent.isStopped = false;
                if (IsAtDestination())
                {
                    currentState = State.BEFORE_BITE;
                    biteTimer = beforeBiteTime;
                    //Debug.Log("state: " + currentState);
                }
                else
                {
                    agent.destination = player.position;
                }
                break;
            case State.BEFORE_BITE:
                agent.isStopped = true;
                biteTimer -= Time.deltaTime;
                if (biteTimer <= 0f)
                {
                    Bite();
                    biteTimer = afterBiteTime;
                    currentState = State.AFTER_BITE;
                }
                break;
            case State.AFTER_BITE:
                biteTimer -= Time.deltaTime;
                if (biteTimer <= 0f)
                {
                    currentState = State.CHASING;
                    agent.destination = player.position;
                }
                break;
            default:
                break;
        }
    }
    private bool IsAtDestination()
    {
        return agent.remainingDistance <= biteDistance;
    }

    private void Bite()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= biteDistance)
        {
            var playerTarget = player.GetComponent<Target>();
            if (playerTarget != null)
            {
                playerTarget.TakeDamage(damage);
            }
        }
    }

    public void OnEnemyDies()
    {
        diesEvent.Raise(new EnemyEventParam(transform.position, cost));
    }
}
