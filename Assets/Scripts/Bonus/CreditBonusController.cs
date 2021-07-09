using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditBonusController : AbstractBonusController
{

    public int amount;

    private ProgressionManager progressionManager;

    public override bool OnPickUp()
    {
        progressionManager.progressionHolder.moneyCount += amount;
        progressionManager.WriteToSaveFile(); // Async!!!
        return true;
    }

    public void SetProgressionManager(ProgressionManager progressionManager)
    {
        this.progressionManager = progressionManager;
    }
}
