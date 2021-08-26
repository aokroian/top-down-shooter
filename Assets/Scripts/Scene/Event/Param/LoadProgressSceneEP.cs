using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadProgressSceneEP
{
    public SceneEnum scene;
    public bool complete;

    public LoadProgressSceneEP(SceneEnum scene, bool complete)
    {
        this.scene = scene;
        this.complete = complete;
    }
}
