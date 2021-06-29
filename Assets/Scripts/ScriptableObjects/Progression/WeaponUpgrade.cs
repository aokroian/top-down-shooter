using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/WeaponUpgrade", order = 51)]
public class WeaponUpgrade : AbstractUpgrade
{
    public WeaponEnum weapon;
    public WeaponUpgradeType weaponUpgradeType;
    public int value;

    public WeaponUpgrade()
    {
        upgradeType = UpgradeType.WEAPON_UPGRADE;
    }
    
}
