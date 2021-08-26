using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngameProgressionManager : MonoBehaviour
{
    public WeaponController pistolPrefab;
    public WeaponController[] otherWeaponPrefabs;
    public ProgressionHolder progressionHolder;
    public GameObject weaponPreview;
    public Camera weaponPreviewCamera;

    private GameObject[] currentWeapons;

    
    public GameObject[] GetWeapons()
    {
        var selected = progressionHolder.GetSelected();
        HashSet<GameObject> result = new HashSet<GameObject>();

        var pistolClone = Instantiate(pistolPrefab.gameObject, weaponPreview.transform.position + Vector3.up, Quaternion.identity, weaponPreview.transform);
        var pistolController = pistolClone.GetComponent<WeaponController>();
        var pistolUpgrade = progressionHolder.GetPurchasedUpgrades().FirstOrDefault(e => e.upgradeType == UpgradeType.WEAPON_UPGRADE && ((WeaponUpgrade) e).weapon == WeaponEnum.PISTOL && ((WeaponUpgrade)e).isRoot);
        if (pistolUpgrade != null) {
            pistolController.shotDamage *= CalcDamageMultiplier(pistolUpgrade);
        }
        result.Add(pistolClone);

        foreach (WeaponUnlock upgrade in selected) {
            var prefab = otherWeaponPrefabs.First(e => e.type == upgrade.weapon);
            var clone = Instantiate(prefab.gameObject, weaponPreview.transform.position + Vector3.up, Quaternion.identity, weaponPreview.transform);
            var controller = clone.GetComponent<WeaponController>();
            controller.shotDamage *= CalcDamageMultiplier(upgrade);
            result.Add(clone);
        }
        currentWeapons = result.ToArray();
        return currentWeapons;
    }
    
    // TODO: Pattern
    // Now ignore upgrade type!!
    private float CalcDamageMultiplier(AbstractUpgrade weapon)
    {
        float result = 1f;
        WeaponUpgrade upgrade = weapon.upgradeType == UpgradeType.WEAPON_UPGRADE ? (WeaponUpgrade) weapon : (WeaponUpgrade) weapon.children[0];
        while (upgrade != null && progressionHolder.IsPurchased(upgrade))
        {
            result += upgrade.value / 100f;
            upgrade = upgrade.children.Length == 0 ? null : (WeaponUpgrade) upgrade.children[0];
        }

        return result;
    }

    public void WeaponChanged(GameObject weapon)
    {
        foreach (GameObject w in currentWeapons)
        {
            if (w == weapon)
            {
                float pistolSize = pistolPrefab.GetComponent<MeshRenderer>().bounds.size.z;

                w.transform.position = weaponPreview.transform.position;
                Vector3 center = w.GetComponentInChildren<MeshRenderer>().bounds.center;
                Vector3 cameraPos = new Vector3(0.8f, 0.18f, 0.25f) * (w.GetComponentInChildren<MeshRenderer>().bounds.size.z / pistolSize);

                weaponPreviewCamera.transform.position = center + cameraPos;
                weaponPreviewCamera.transform.LookAt(center);
            } else {
                w.transform.position = weaponPreview.transform.position + Vector3.up;
            }
        }
    }
}
