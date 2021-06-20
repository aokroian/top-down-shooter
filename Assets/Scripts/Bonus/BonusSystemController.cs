using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSystemController : MonoBehaviour
{

    public GameObject player;
    public GameObject healthPrefab;

    public BonusSpawnParams health;

    void Start()
    {
    }

    
    void Update()
    {
        
    }

    public void EnemyDies(EnemyEventParam param)
    {
        List<GameObject> spawnedBonuses = new List<GameObject>();

        Debug.Log("EnemyDies event executed!! Chance: " + health.GetChance(param.cost));
        if (health.IsShouldSpawn(param.cost))
        {
            Debug.Log("Bonus Spawned. Pos: " + param.position);
            Vector3 pos = CalcPosition(param.position, spawnedBonuses);
            spawnedBonuses.Add(SpawnBonus(healthPrefab, pos)); 
        }
    }

    private Vector3 CalcPosition(Vector3 initPos, List<GameObject> spawned)
    {
        // TODO
        return initPos;
    }

    private GameObject SpawnBonus(GameObject prefab, Vector3 pos)
    {
        return Instantiate(prefab, pos, prefab.transform.rotation, transform);
    }    
}
