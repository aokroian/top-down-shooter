using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject settingsScreen;
    public GameObject upgradeScreen;

    public ChangeSceneEvent changeSceneEvent;
    public LoadProgressSceneEvent loadProgressSceneEvent;

    private void Awake()
    {
        Vibration.Init();
    }

    private void Start()
    {
        settingsScreen.GetComponent<SettingsScreen>().SetBackAction(ToTitleScreen);
        LoadComplete();
    }

    public void ToTitleScreen()
    {
        Debug.Log("ToTitleScreen");
        titleScreen.SetActive(true);
        settingsScreen.SetActive(false);
        upgradeScreen.SetActive(false);
    }

    public void ToSettingsScreen()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(true);
        upgradeScreen.SetActive(false);
    }

    public void ToUpgradeScreen()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(false);
        upgradeScreen.SetActive(true);
    }

    public void StartGame()
    {
        Debug.Log("START GAME");
        var param = new ChangeSceneEventParam(SceneEnum.GAME, SceneEnum.TITLE, false);
        changeSceneEvent.Raise(param);
    }

    private void LoadComplete()
    {
        var param = new LoadProgressSceneEP(SceneEnum.TITLE, true);
        loadProgressSceneEvent.Raise(param);
    }
}
