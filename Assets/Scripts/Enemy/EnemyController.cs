using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, EnemyProperties
{
    public enum State
    {
        IDLE,
        CHASING,
        BEFORE_BITE,
        AFTER_BITE
    }

    [SerializeField]
    private int _cost;
    public int cost { get => _cost; set => _cost = value; }

    public State defaultState = State.IDLE;
    public float visionRange = 15.0f;

    public float damage = 20f;
    public float biteDistance = 1.0f;
    public float beforeBiteTime = 0.2f;
    public float afterBiteTime = 0.8f;

    public float posDetectionDelay = 0.1f;
    public float minPosChangeForAnimation = 0.1f;
    public float biteTimer;

    public GameObject aimAtPoint;


    public EnemyDiesEvent diesEvent;

    private State currentState;
    private Transform player;

    private NavMeshAgent agent;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (aimAtPoint != null)
        {
            aimAtPoint.transform.position = player.transform.position;
        }
        /*
        // rotate towards player
        Vector3 lTargetDir = player.transform.position - transform.position;
        lTargetDir.y = 0.0f;
        gameObject.transform.rotation = Quaternion.LookRotation(lTargetDir);
        */
        Vector3 velocity = agent.velocity;
        Vector3 globalMovement = new Vector3(velocity.x, 0.0f, velocity.z);
        Vector3 localMovement = gameObject.transform.InverseTransformDirection(globalMovement);
        gameObject.GetComponent<Animator>().SetFloat("local Z speed", localMovement.z);
        gameObject.GetComponent<Animator>().SetFloat("local X speed", localMovement.x);
        
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
                    currentState = State.CHASING;
                    //Debug.Log("state: " + currentState);
                }
                break;
            case State.CHASING:
                agent.isStopped = false;
                agent.destination = player.position;
                if (IsAtDestination())
                {
                    Debug.DrawLine(agent.destination, agent.destination + (Vector3.up * 2), Color.red, 10f);
                    currentState = State.BEFORE_BITE;
                    biteTimer = beforeBiteTime;
                    //Debug.Log("state: " + currentState);
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

        Debug.DrawLine(agent.destination, agent.destination + (Vector3.up * 2), Color.green, 1f);
    }
    private bool IsAtDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= biteDistance;
        //return Vector3.Distance(transform.position, player.transform.position) <= biteDistance;
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

    public void Aware(AwareEventParam param)
    {
        if (currentState == State.IDLE && Vector3.Distance(param.position, transform.position) <= param.distance)
        {
            currentState = State.CHASING;
        }
    }
}
