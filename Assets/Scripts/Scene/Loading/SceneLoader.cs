using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    // To use interface need to make custom PropertyDrawer
    public MonoBehaviour[] loadables;
    public SceneEnum sceneIndex;
    public LoadProgressSceneEvent loadProgressSceneEvent;

    private int currentCost;

    private void Start()
    {
        // Must be in different cycles
        CalcOverallCost();
        StartLoading();
    }

    private void CalcOverallCost()
    {
        foreach (Loadable part in loadables)
        {
            currentCost += part.loadCost;
        }
    }

    private void StartLoading()
    {
        foreach (Loadable part in loadables)
        {
            part.Load(() => PartLoaded(part.loadCost));
        }
    }

    private void PartLoaded(int cost)
    {
        currentCost -= cost;
        var param = new LoadProgressSceneEP(sceneIndex, currentCost == 0);
        loadProgressSceneEvent.Raise(param);
    }
}
