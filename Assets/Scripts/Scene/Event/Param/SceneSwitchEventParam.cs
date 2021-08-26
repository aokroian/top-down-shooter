using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitchEventParam
{
    public enum SceneLoadStateEnum
    {
        STARTED = 0,
        LOADED = 1
    }

    public SceneLoadStateEnum state;
    public SceneEnum scene;
    public SceneEnum previousScene;

    public SceneSwitchEventParam(SceneLoadStateEnum state, SceneEnum scene, SceneEnum previousScene)
    {
        this.state = state;
        this.scene = scene;
        this.previousScene = previousScene;
    }
}
