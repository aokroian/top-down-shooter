using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBonusController : AbstractBonusController
{
    public int amount;
    public AmmoType ammoType;

    public override bool OnPickUp()
    {
        int added = player.GetComponent<PlayerController>().ammoController.AddAmmo(ammoType, amount);
        return added != 0;
    }
}
