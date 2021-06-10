using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemySpawner))]
public class EnemySpawnerManager: MonoBehaviour
{
    public GameObject player;

    [Tooltip("Distance player should pass to enemies spawn")]
    public float spawnDistancePassed = 10.0f;

    [Tooltip("Min distance from player to spawn point")]
    public float minSpawnPointDistance = 30.0f;

    [Tooltip("Max distance from player to spawn point")]
    public float maxSpawnPointDistance = 40.0f;

    [Range(0.0f, 360.0f)]
    [Tooltip("Cone where spawn point can be")]
    public float spawnPointAngle = 90.0f;

    public int startSpawnCost = 3;
    public float spawnCostDistanceMultiplier = 0.01f;

    public float waveDistancePassed = 30.0f;
    public float waveAngle = 90.0f;

    public int startSpawnWaveCost = 5;
    public float waveCostDistanceMultiplier = 0.1f;

    private EnemySpawner spawner;

    private Vector3 prevSpawnPosition;
    private float halfSpawnPointAngle;

    private int nextWaveCount = 1;

    void Start()
    {
        spawner = GetComponent<EnemySpawner>();
        prevSpawnPosition = player.transform.position;
        halfSpawnPointAngle = spawnPointAngle / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, prevSpawnPosition) > spawnDistancePassed)
        {
            Vector3 angle = (player.transform.position - prevSpawnPosition).normalized;
            int spawnCost = Mathf.FloorToInt(startSpawnCost + Vector3.Distance(Vector3.zero, player.transform.position) * spawnCostDistanceMultiplier);
            SpawnAtPoint(angle, spawnCost, 1, 2);

            prevSpawnPosition = player.transform.position;
        }

        if (Vector3.Distance(player.transform.position, Vector3.zero) > nextWaveCount * waveDistancePassed)
        {
            Vector3 angle = player.transform.position.normalized;
            int waveSpawnCost = Mathf.FloorToInt(startSpawnWaveCost + Vector3.Distance(Vector3.zero, player.transform.position) * waveCostDistanceMultiplier);
            SpawnWave(angle, waveSpawnCost, 1, 2);

            nextWaveCount++;
        }
    }

    private void SpawnAtPoint(Vector3 towards, int spawnCost, int minEnemyCost, int maxEnemyCost)
    {
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(0 - halfSpawnPointAngle, halfSpawnPointAngle), Vector3.up);
        Vector3 randomAngle = rotation * towards;
        Vector3 spawnPoint = randomAngle * Random.Range(minSpawnPointDistance, maxSpawnPointDistance) + player.transform.position;
        //Debug.DrawLine(new Vector3(spawnPoint.x, 0.0f, spawnPoint.z), new Vector3(spawnPoint.x, 20.0f, spawnPoint.z), Color.red, 100.0f);

        spawner.Spawn(spawnPoint, spawnCost, minEnemyCost, maxEnemyCost);
    }

    private void SpawnWave(Vector3 towards, int spawnCost, int minEnemyCost, int maxEnemyCost)
    {
        spawner.SpawnWave(player.transform.position, towards, waveAngle, minSpawnPointDistance, maxSpawnPointDistance, spawnCost, minEnemyCost, maxEnemyCost);
    }

    public void Spawn(GameObject groundTile)
    {
        //spawner.SpawnByGround(groundTile, 5, 1, 2);
    }

}
