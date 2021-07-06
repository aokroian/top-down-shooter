using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public ProgressionHolder progressionHolder;

    private SaveLoadController saveLoadController;
    private JsonConverter jsonConverter;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (saveLoadController == null)
        {
            jsonConverter = new JsonConverter();
            saveLoadController = new SaveLoadController(jsonConverter);
        }
    }

    public void LoadFromSaveFile()
    {
        var saveData = saveLoadController.LoadProgression();
        progressionHolder.moneyCount = saveData.money;
        progressionHolder.SetPurchasedUpgrades(saveData.upgrades);
    }

    public void WriteToSaveFile() {
        saveLoadController.SaveProgression(progressionHolder.moneyCount, progressionHolder.GetPurchasedUpgradesId());
    }

}
