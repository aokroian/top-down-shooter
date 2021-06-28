using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSystemController : MonoBehaviour
{

    public GameObject player;

    //public GameObject healthPrefab;
    public BonusSpawnParams health;

    //public GameObject[] ammoPrefabs;
    public BonusSpawnParams[] ammoParams;

    private Dictionary<AmmoType, BonusSpawnParams> actualAmmoParams = new Dictionary<AmmoType, BonusSpawnParams>();

    void Start()
    {
        FillActualAmmoParams();
    }

    private void FillActualAmmoParams()
    {
        var types = player.GetComponent<PlayerController>().GetAmmoTypes();
        foreach (AmmoType t in types)
        {
            BonusSpawnParams param = null;
            foreach (BonusSpawnParams bsp in ammoParams)
            {
                param = null;
                if (bsp.bonusPrefab.GetComponent<AmmoBonusController>().ammoType == t)
                {
                    param = bsp;
                    break;
                }
            }
            if (param != null) {
                actualAmmoParams.Add(t, param);
            }
        }
    }

    public void EnemyDies(EnemyEventParam param)
    {
        List<GameObject> spawnedBonuses = new List<GameObject>();

        //Debug.Log("EnemyDies event executed!! Chance: " + health.GetChance(param.cost));
        if (health.IsShouldSpawn(param.cost))
        {
            Vector3 pos = CalcPosition(param.position, spawnedBonuses);
            spawnedBonuses.Add(PrepareAndSpawnBonus(health.bonusPrefab, pos));
        }

        foreach (KeyValuePair<AmmoType, BonusSpawnParams> entity in actualAmmoParams)
        {
            if (entity.Value.IsShouldSpawn(param.cost))
            {
                Debug.Log("Spawned ammo: " + entity.Key);
                Vector3 pos = CalcPosition(param.position, spawnedBonuses);
                spawnedBonuses.Add(PrepareAndSpawnBonus(entity.Value.bonusPrefab, pos));
            }
        }
    }

    private Vector3 CalcPosition(Vector3 initPos, List<GameObject> spawned)
    {
        // TODO
        Vector3 delta = Vector3.zero;
        if (spawned.Count > 0)
        {
            delta = new Vector3(spawned[spawned.Count - 1].GetComponent<Collider>().bounds.size.x, 0f, 0f);
        }
        return initPos + delta;
    }

    private GameObject PrepareAndSpawnBonus(GameObject prefab, Vector3 pos)
    {
        GameObject bonus = Instantiate(prefab, pos, prefab.transform.rotation, transform);
        bonus.GetComponent<AbstractBonusController>().player = player;
        return bonus;
    }
}
