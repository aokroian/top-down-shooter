using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSystemController : MonoBehaviour
{

    public GameObject player;
    public GameObject healthPrefab;

    [Header("Base chances")]
    public float healthBase;

    [Header("Additional chance for every enemy killed and not get (Percent from base chance!)")]
    public float healthNotGetAdditionalPercent;

    [Header("Additional chance for every enemy cost exceeding 1 (Percent from base chance!)")]
    public float healthEnemyCostAdditionalPercent;

    private float healthNotGet;

    private float healthEnemyCost;

    private float healthNotGetCurrent;

    void Start()
    {
        healthNotGet = (healthBase / 100) * healthNotGetAdditionalPercent;
        healthEnemyCost = (healthBase / 100) * healthEnemyCostAdditionalPercent;
    }

    
    void Update()
    {
        
    }

    public void EnemyDies(EnemyEventParam param)
    {
        Debug.Log("EnemyDies event executed!!");
        float healthChance = GetHealthChance(param.cost);
    }

    private GameObject SpawnBonusOrNull(GameObject prefab, float chance, Vector3 pos)
    {
        GameObject result = null;
        if (Random.value <= chance)
        {
            result = Instantiate(prefab, pos, prefab.transform.rotation, transform);
        }
        return result;
    }

    public float GetHealthChance(float cost)
    {
        return healthBase + healthNotGetCurrent + (healthEnemyCost * cost);
    }
}
