using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Upgrade/WeaponUpgrade", order = 51)]
public class WeaponUpgrade : AbstractUpgrade
{
    public WeaponEnum weapon;
    public WeaponUpgradeType weaponUpgradeType;
    public int value;

    // Must fill at ProgressionHolder!!!
    [HideInInspector]
    public int tier;

    public WeaponUpgrade()
    {
        upgradeType = UpgradeType.WEAPON_UPGRADE;
    }
    
}
