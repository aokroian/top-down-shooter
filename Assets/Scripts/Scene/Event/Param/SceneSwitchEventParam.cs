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
}
