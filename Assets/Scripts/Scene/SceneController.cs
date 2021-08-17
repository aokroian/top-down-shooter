using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject loadingScreen;
    public SceneSwitchEvent sceneSwitchEvent;

    private ChangeSceneEventParam currentChangeSceneEventParam;

    private void Awake()
    {
        var param = new ChangeSceneEventParam(SceneEnum.TITLE, SceneEnum.NULL, true);
        ChangeScene(param);
    }

    public void ChangeScene(ChangeSceneEventParam param)
    {
        currentChangeSceneEventParam = param;
        var switchParam = new SceneSwitchEventParam(SceneSwitchEventParam.SceneLoadStateEnum.STARTED, param.scene, param.sceneToUnload);
        sceneSwitchEvent.Raise(switchParam);
        SetLoadingScreenActive(true);
        StartCoroutine(SwitchSceneCoroutine(param));
    }

    // Should be executed next frame!
    private IEnumerator SwitchSceneCoroutine(ChangeSceneEventParam param)
    {
        yield return null;
        if (param.sceneToUnload != SceneEnum.NULL) {
            SceneManager.UnloadSceneAsync((int)param.sceneToUnload);
        }
        var operation = SceneManager.LoadSceneAsync((int)param.scene, LoadSceneMode.Additive);
        operation.completed += (op) => UnitySceneLoaded(param);
    }

    public void SetLoadProgress(LoadProgressSceneEP param)
    {
        if (param.complete)
        {
            SceneLoadCompleted(currentChangeSceneEventParam);
            if (param.onSceneActive != null)
            {
                param.onSceneActive();
            }
        }
    }

    private void UnitySceneLoaded(ChangeSceneEventParam param)
    {
        if (param.showAfterLoad)
        {
            SceneLoadCompleted(param);
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)param.scene));
    }

    private void SceneLoadCompleted(ChangeSceneEventParam param)
    {
        SetLoadingScreenActive(false);
        var switchParam = new SceneSwitchEventParam(SceneSwitchEventParam.SceneLoadStateEnum.STARTED, param.scene, param.sceneToUnload);
        sceneSwitchEvent.Raise(switchParam);
    }

    private void SetLoadingScreenActive(bool active)
    {
        loadingScreen.SetActive(active);
    }
}
