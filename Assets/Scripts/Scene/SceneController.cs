using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject loadingScreen;

    private void Awake()
    {
        var operation = SceneManager.LoadSceneAsync((int)SceneEnum.TITLE, LoadSceneMode.Additive);
        operation.completed += (op) => SceneLoaded((int)SceneEnum.TITLE, true);
    }

    public void ChangeScene(ChangeSceneEventParam param)
    {
        Debug.Log("Before Loading screen");
        SetLoadingScreenActive(true);
        Debug.Log("After Loading screen");
        StartCoroutine(SwitchSceneCoroutine(param));
    }

    // Should be executed next frame!
    private IEnumerator SwitchSceneCoroutine(ChangeSceneEventParam param)
    {
        yield return null;
        SceneManager.UnloadSceneAsync((int)param.sceneToUnload);
        var operation = SceneManager.LoadSceneAsync((int)param.scene, LoadSceneMode.Additive);
        operation.completed += (op) => SceneLoaded((int)param.scene, param.showAfterLoad);
    }

    public void SetLoadProgress(LoadProgressSceneEP param)
    {
        if (param.complete)
        {
            SetLoadingScreenActive(false);
            if (param.onSceneActive != null)
            {
                param.onSceneActive();
            }
        }
    }

    private void SceneLoaded(int scene, bool showAfterLoad)
    {
        if (showAfterLoad)
        {
            SetLoadingScreenActive(false);
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(scene));
    }

    private void SetLoadingScreenActive(bool active)
    {
        loadingScreen.SetActive(active);
    }
}
