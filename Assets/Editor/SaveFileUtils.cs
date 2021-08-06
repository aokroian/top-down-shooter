using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class SaveFileUtils
{
    private static SaveLoadController slController;

    private static SaveLoadController getController()
    {
        if (slController == null)
        {
            slController = new SaveLoadController(new JsonConverter());
        }
        return slController;
    }

    [MenuItem("Custom/Save data/Delete save file")]
    public static void DeleteSaveFile()
    {
        File.Delete(getController().fileFullPath);
        Debug.Log("Save file deleted");
    }

    [MenuItem("Custom/Save data/Add 1000 to save file")]
    public static void AddMoneyToSaveFile()
    {
        var data = getController().LoadProgression();
        var current = data.money + 1000;
        getController().SaveProgression(current, data.upgrades, data.selected, data.topScore);
        Debug.Log("Money added. Current: " + current);
    }
}