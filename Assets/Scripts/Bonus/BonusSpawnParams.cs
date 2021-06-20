using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BonusSpawnParams
{
    // Percent
    public float baseChance;

    // Percent from base chance!
    public float notGetPercent;
    public float enemyCostAdditionalPercent;

    private int notGetCount;

    public bool IsShouldSpawn(float enemyCost)
    {
        var result = Random.value <= GetChance(enemyCost);
        if (result)
        {
            notGetCount = 0;
        } else
        {
            notGetCount ++;
        }
        return result;
    }

    /// <summary>
    /// </summary>
    /// <returns>0..1</returns>
    public float GetChance(float enemyCost)
    {
        return (baseChance + PercentToChance(notGetPercent * notGetCount) + PercentToChance(enemyCostAdditionalPercent * (enemyCost - 1))) / 100.0f;
    }

    private float PercentToChance(float percent)
    {
        return (baseChance / 100.0f) * percent;
    }
}
