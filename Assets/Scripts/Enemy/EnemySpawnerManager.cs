using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemySpawner))]
public class EnemySpawnerManager: MonoBehaviour
{
    public GameObject player;

    private EnemySpawner spawner;
    
    void Start()
    {
        spawner = GetComponent<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(GameObject groundTile)
    {
        spawner.Spawn(groundTile, 5, 1, 2);
    }

}
