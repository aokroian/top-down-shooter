using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneEventParam
{
    public SceneEnum scene { get; private set; }
    public SceneEnum sceneToUnload { get; private set; }
    public bool showAfterLoad { get; private set; }
    public Action onLoad { get; private set; }
    public string param { get; private set; }

    public ChangeSceneEventParam(SceneEnum scene, SceneEnum sceneToUnload, bool showAfterLoad, Action onLoad = null, string param = null)
    {
        this.scene = scene;
        this.sceneToUnload = sceneToUnload;
        this.showAfterLoad = showAfterLoad;
        this.onLoad = onLoad;
        this.param = param;
    }
}
