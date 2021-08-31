using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Upgrade/PlayerUpgrade", order = 53)]
public class PlayerUpgrade : AbstractUpgrade
{
    public PlayerUpgradeType playerUpgradeType;
    public int value;

    public PlayerUpgrade()
    {
        upgradeType = UpgradeType.PLAYER_UPGRADE;
    }
}
