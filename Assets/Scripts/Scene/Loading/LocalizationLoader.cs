using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationLoader : MonoBehaviour, Loadable
{
    public int _loadCost = 2;
    public int loadCost => _loadCost;
    public LocalizationTableHolder localizationTableHolder;

    public void Load(Action onLoad)
    {
        localizationTableHolder.SetListener(() => {
            localizationTableHolder.removeListener();
            onLoad();
        });
        localizationTableHolder.Init();
    }
}
