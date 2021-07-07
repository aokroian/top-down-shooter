using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngameProgressionManager : MonoBehaviour
{
    public WeaponController pistolPrefab;
    public WeaponController[] otherWeaponPrefabs;
    public ProgressionHolder progressionHolder;
    public ProgressionManager progressionManager;

    
    public GameObject[] GetWeapons()
    {
#if UNITY_EDITOR
        progressionManager.LoadFromSaveFile();
#endif
        var selected = progressionHolder.GetSelected();
        HashSet<GameObject> result = new HashSet<GameObject>();

        // TODO: pistol upgrades!!!
        result.Add(Instantiate(pistolPrefab.gameObject, new Vector3(0f, -10f, 0f), Quaternion.identity));
        foreach (WeaponUnlock upgrade in selected) {
            var prefab = otherWeaponPrefabs.First(e => e.type == upgrade.weapon);
            var clone = Instantiate(prefab.gameObject, new Vector3(0f, -10f, 0f), Quaternion.identity);
            var controller = clone.GetComponent<WeaponController>();
            controller.shotDamage *= CalcDamageMultiplier(upgrade);
            result.Add(clone);
        }

        return result.ToArray();
    }
    
    // TODO: Pattern
    // Now ignore upgrade type!!
    private float CalcDamageMultiplier(WeaponUnlock weapon)
    {
        float result = 1f;
        WeaponUpgrade upgrade = (WeaponUpgrade) weapon.children[0];
        while (upgrade != null && progressionHolder.IsPurchased(upgrade))
        {
            result += upgrade.value / 100f;
            upgrade = upgrade.children.Length == 0 ? null : (WeaponUpgrade) upgrade.children[0];
        }

        return result;
    }
}
