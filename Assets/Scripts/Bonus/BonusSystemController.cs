using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSystemController : MonoBehaviour
{

    public GameObject player;

    public ProgressionManager progressionManager;

    //public GameObject healthPrefab;
    public BonusSpawnParams health;
    public BonusSpawnParams credits;

    //public GameObject[] ammoPrefabs;
    public BonusSpawnParams[] ammoParams;
    public LocalizationTableHolder localizationTableHolder;


    private Dictionary<AmmoType, BonusSpawnParams> actualAmmoParams = new Dictionary<AmmoType, BonusSpawnParams>();

    void Start()
    {
        localizationTableHolder.Init();
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
        SpawnIfNeed(param, health, spawnedBonuses);
        var creditObject = SpawnIfNeed(param, credits, spawnedBonuses);
        if (creditObject != null) {
            creditObject.GetComponent<CreditBonusController>().SetProgressionManager(progressionManager);
        }

        foreach (KeyValuePair<AmmoType, BonusSpawnParams> entity in actualAmmoParams)
        {
            SpawnIfNeed(param, entity.Value, spawnedBonuses);
        }
    }
    
    private GameObject SpawnIfNeed(EnemyEventParam eventParam, BonusSpawnParams bonusParam, List<GameObject> spawnedBonuses)
    {
        GameObject result = null;
        if (bonusParam.IsShouldSpawn(eventParam.cost))
        {
            Vector3 pos = CalcPosition(eventParam.position, spawnedBonuses);
            result = PrepareAndSpawnBonus(bonusParam.bonusPrefab, pos);
            spawnedBonuses.Add(result);
        }
        return result;
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
        AbstractBonusController bonusController = bonus.GetComponent<AbstractBonusController>();
        bonusController.player = player;

        bonusController.SetLocalizationTableHolder(localizationTableHolder);
        return bonus;
    }
}
