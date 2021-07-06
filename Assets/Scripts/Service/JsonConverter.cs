using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonConverter
{
    public SaveLoadController.SaveData Parse(string json)
    {
        return JsonUtility.FromJson<SaveLoadController.SaveData>(json);
    }

    public string Serialize(SaveLoadController.SaveData data)
    {
        return JsonUtility.ToJson(data);
    }
}
