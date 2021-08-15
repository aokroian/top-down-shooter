using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneEventParam
{
    public SceneEnum scene { get; private set; }
    public SceneEnum sceneToUnload { get; private set; }
    public bool showAfterLoad { get; private set; }

    public ChangeSceneEventParam(SceneEnum scene, SceneEnum sceneToUnload, bool showAfterLoad)
    {
        this.scene = scene;
        this.sceneToUnload = sceneToUnload;
        this.showAfterLoad = showAfterLoad;
    }
}
