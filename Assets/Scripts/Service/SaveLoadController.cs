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
    }

    private string fileFullPath = Application.persistentDataPath + "/save.dat";

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
        return result;
    }

    public void SaveProgression(int money, int[] upgrades)
    {
        SaveData data = new SaveData();
        data.money = money;
        data.upgrades = upgrades;
        SaveProgression(data);
    }

    public void SaveProgression(SaveData data)
    {
        string json = converter.Serialize(data);
        File.WriteAllText(fileFullPath, json);
    }
}
