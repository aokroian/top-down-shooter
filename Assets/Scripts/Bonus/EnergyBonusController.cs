using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBonusController : AbstractBonusController
{
    public float amount = 20f;

    private Target playerTarget;

    private void Start()
    {
        playerTarget = player.GetComponent<Target>();
    }

    public override bool OnPickUp()
    {
        return playerTarget.Heal(amount);
    }

    public override bool CanPickUp()
    {
        return playerTarget.CanHeal();
    }

    public override string GetPickupText()
    {
        string localizedString = localizationTableHolder.currentTable.GetEntry(pickupString).GetLocalizedString();
        return "+" + amount + " " + localizedString;
    }
}
