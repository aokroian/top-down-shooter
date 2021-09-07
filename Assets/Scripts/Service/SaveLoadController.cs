using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadController
{
    [System.Serializable]
    public struct SaveData
    {
        public int money;
        public int[] upgrades;
        public int[] selected;
        public int topScore;
        public int[] playerUpgrades;
        public int exp;
    }

    public string fileFullPath { get; } = Application.persistentDataPath + "/save.dat";

    private JsonConverter converter;

    public SaveLoadController(JsonConverter converter)
    {
        this.converter = converter;
    }

    public SaveData LoadProgression()
    {
        SaveData result = new SaveData();

        if (File.Exists(fileFullPath))
        {
            string json = File.ReadAllText(fileFullPath);
            result = converter.Parse(json);
        }

        if (result.upgrades == null)
        {
            result.upgrades = new int[0];
        }
        if (result.selected == null)
        {
            result.selected = new int[0];
        }
        if (result.playerUpgrades == null)
        {
            result.playerUpgrades = new int[0];
        }
        return result;
    }

    public void SaveProgression(int money, int[] upgrades, int[] selected, int topScore, int[] playerUpgrades, int exp)
    {
        SaveData data = new SaveData();
        data.money = money;
        data.upgrades = upgrades;
        data.selected = selected;
        data.topScore = topScore;
        data.playerUpgrades = playerUpgrades;
        data.exp = exp;
        SaveProgression(data);
    }

    public void SaveProgression(SaveData data)
    {
        string json = converter.Serialize(data);
        Debug.Log("Save: " + json);
        File.WriteAllText(fileFullPath, json);
    }

    public void DeleteSaveFile()
    {
        File.Delete(fileFullPath);
    }
}
