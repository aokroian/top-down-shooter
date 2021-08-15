using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadProgressSceneEP
{
    public bool complete;
    public Action onSceneActive;

    public LoadProgressSceneEP(bool complete, Action onSceneActive = null)
    {
        this.complete = complete;
        this.onSceneActive = onSceneActive;
    }
}
