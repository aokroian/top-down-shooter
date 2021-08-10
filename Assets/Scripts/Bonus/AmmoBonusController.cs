using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBonusController : AbstractBonusController
{
    public int amount;
    public AmmoType ammoType;

    private PlayerAmmoController ammoController;

    private void Start()
    {
        ammoController = player.GetComponent<PlayerController>().ammoController;
    }

    public override bool OnPickUp()
    {
        int added = ammoController.AddAmmo(ammoType, amount);
        return added != 0;
    }

    public override bool CanPickUp()
    {
        return !ammoController.IsAmmoFull(ammoType);
    }

    public override string GetPickupText()
    {
        return "+" + amount + " AMMO_TYPE_PLACEHOLDER";
    }
}
