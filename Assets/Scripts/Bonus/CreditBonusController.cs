using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditBonusController : AbstractBonusController
{

    public int amount;

    public GameEvent progressionChangeEvent;
    public ProgressionHolder progressionHolder;

    public override bool OnPickUp()
    {
        progressionHolder.moneyCount += amount;
        progressionChangeEvent.Raise();
        return true;
    }

    public override bool CanPickUp()
    {
        return true;
    }

    public override string GetPickupText()
    {
        string localizedString = localizationTableHolder.currentTable.GetEntry(pickupString).GetLocalizedString();
        return "+" + amount + " " + localizedString;
    }
}
