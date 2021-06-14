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

    private State currentState;
    private Transform player;

    private NavMeshAgent agent;

    public float biteTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
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
                    Debug.Log("state: " + currentState);
                }
                break;
            case State.CHASING:
                agent.isStopped = false;
                if (IsAtDestination())
                {
                    currentState = State.BEFORE_BITE;
                    biteTimer = beforeBiteTime;
                    Debug.Log("state: " + currentState);
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
}
