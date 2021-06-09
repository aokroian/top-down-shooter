using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{

    // TODO!!!!!!!!
    public GameObject enemyPrefab;

    [Range(1, 50)]
    public int attemptCount = 10;

    public float minDistanceMultiplier;
    public float maxDistanceMultiplier;

    private int costLeft;
    private float navMeshSearchMaxDistance = 5.0f; // could change. Depends on obstacle size

    private class SpawnPoint
    {
        public Vector2 pos;
        public float radius;
        public bool active;
        
        public SpawnPoint(Vector2 pos, float radius)
        {
            this.pos = pos;
            this.radius = radius;
            this.active = true;
        }
    }

    // TODO: Change parameters to SpawnConfig?
    public void SpawnByGround(GameObject groundTile, int groupCost, int minEnemyCost, int maxEnemyCost)
    {
        var bounds = groundTile.GetComponent<MeshRenderer>().bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        Spawn(new Vector3(x, 0.0f, z), groupCost, minEnemyCost, maxEnemyCost);
    }

    public void Spawn(Vector3 point, int groupCost, int minEnemyCost, int maxEnemyCost)
    {
        costLeft = groupCost;
        

        NavMeshHit hit;
        NavMesh.SamplePosition(point, out hit, navMeshSearchMaxDistance, 1); // 1: walkable
        if (!hit.hit)
        {
            Debug.LogError("NOT HIT!!! ERROR!!");
            return;
        }

        List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
        var firstEnemy = GetEnemyPrefabByCost(minEnemyCost, maxEnemyCost);
        spawnPoints.Add(SpawnEnemySubtractCost(new Vector2(hit.position.x, hit.position.z), firstEnemy));

        while (costLeft >= minEnemyCost)
        {
            bool found = false;
            var activePoint = spawnPoints.FirstOrDefault(p => p.active);
            //Debug.Log("radius: " + activePoint.radius + "; active: " + activePoint.active);
            if (activePoint == null)
            {
                Debug.LogError("NO AVAILABLE PLACE FOR SPAWN POINT!!! ERROR!!");
                return;
            }

            var enemy = GetEnemyPrefabByCost(minEnemyCost, maxEnemyCost);
            float radius = enemy.GetComponent<NavMeshAgent>().radius;

            for (int i = 0; i < attemptCount; i++)
            {
                float angle = 2 * Mathf.PI * Random.value;
                float r = radius * (maxDistanceMultiplier - minDistanceMultiplier) * Random.value + radius * minDistanceMultiplier;
                Vector2 candidate = activePoint.pos + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Debug.DrawLine(new Vector3(candidate.x, 1.0f, candidate.y), new Vector3(candidate.x + 0.1f, 1.0f, candidate.y + 0.1f), Color.red, 500.0f, false);
                if (IsInsideNavMesh(candidate) && IsFairEnough(spawnPoints, candidate, radius))
                {
                    found = true;
                    spawnPoints.Add(SpawnEnemySubtractCost(candidate, enemy));
                    break;
                }
            }
            if (!found)
            {
                activePoint.active = false;
            }
        }
    }

    private SpawnPoint SpawnEnemySubtractCost(Vector2 pos, GameObject enemyPrefab)
    {
        var v3pos = new Vector3(pos.x, 0.0f, pos.y);
        var rot = Quaternion.Euler(0.0f, Random.Range(-180.0f, 180.0f), 0.0f); // Rotate towards player maybe?
        //Debug.Log("angle: " + Quaternion.Angle(rot, enemyPrefab.transform.rotation));

        // need return instantiated enemy? Split method in two?
        var enemy = Instantiate(enemyPrefab, v3pos, rot, transform);
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        costLeft -= enemy.GetComponent<EnemyController>().cost;

        return new SpawnPoint(pos, agent.radius);
    }

    private GameObject GetEnemyPrefabByCost(int minEnemyCost, int maxEnemyCost)
    {
        // TODO: Get prefab by cost
        Mathf.Min(costLeft, Random.Range(minEnemyCost, maxEnemyCost));
        return enemyPrefab;
    }

    private bool IsInsideNavMesh(Vector2 candidate)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(candidate.x, 0.0f, candidate.y), out hit, 0.1f, 1);
        //Debug.Log("Hit: " + hit.hit + "; diff x:" + (candidate.x - hit.position.x) + "; z: " + (candidate.y - hit.position.y));
        return hit.hit;
    }

    private bool IsFairEnough(List<SpawnPoint> spawnPoints, Vector2 candidate, float candRadius)
    {
        bool result = true;
        foreach (SpawnPoint p in spawnPoints)
        {
            float radius = Mathf.Max(candRadius, p.radius);
            if (Vector2.Distance(candidate, p.pos) < radius)
            {
                result = false;
                break;
            }
        }
        return result;
    }
}
