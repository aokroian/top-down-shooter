using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadProgressSceneEP
{
    public SceneEnum scene;
    public bool complete;
    public Action onSceneActive;

    public LoadProgressSceneEP(SceneEnum scene, bool complete, Action onSceneActive = null)
    {
        this.scene = scene;
        this.complete = complete;
        this.onSceneActive = onSceneActive;
    }
}
