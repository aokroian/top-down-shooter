using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, EnemyProperties
{
    [SerializeField]
    private int _cost;
    public int cost { get => _cost; set => _cost = value; }

    public bool playerAwared;
    public float visionRange = 15.0f;

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
        if (playerAwared) {
            agent.destination = player.position;
        } else
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
}
