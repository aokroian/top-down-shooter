using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/WeaponUnlock", order = 51)]
public class WeaponUnlock : AbstractUpgrade
{
    public WeaponEnum weapon;

    [HideInInspector]
    public bool selected;

    public WeaponUnlock()
    {
        upgradeType = UpgradeType.WEAPON_UNLOCK;
    }
}
