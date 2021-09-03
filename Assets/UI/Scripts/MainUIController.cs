using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject settingsScreen;
    public GameObject upgradeScreen;
    public GameObject creditsScreen;
    public GameObject playerUpgradeScreen;

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
        titleScreen.SetActive(true);
        settingsScreen.SetActive(false);
        upgradeScreen.SetActive(false);
        creditsScreen.SetActive(false);
        playerUpgradeScreen.SetActive(false);
    }

    public void ToSettingsScreen()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(true);
        upgradeScreen.SetActive(false);
        creditsScreen.SetActive(false);
        playerUpgradeScreen.SetActive(false);
    }

    public void ToUpgradeScreen()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(false);
        upgradeScreen.SetActive(true);
        creditsScreen.SetActive(false);
        playerUpgradeScreen.SetActive(false);
    }

    public void ToCreditsScreen()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(false);
        upgradeScreen.SetActive(false);
        creditsScreen.SetActive(true);
        playerUpgradeScreen.SetActive(false);
    }

    public void ToPlayerUpgradeScreen()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(false);
        upgradeScreen.SetActive(false);
        creditsScreen.SetActive(false);
        playerUpgradeScreen.SetActive(true);
    }

    public void StartGame()
    {
        Debug.Log("START GAME");
        var param = new ChangeSceneEventParam(SceneEnum.GAME, SceneEnum.TITLE, false, active: true);
        changeSceneEvent.Raise(param);
    }

    private void LoadComplete()
    {
        var param = new LoadProgressSceneEP(SceneEnum.TITLE, true);
        loadProgressSceneEvent.Raise(param);
    }

    public void ProcessSceneParam(string param)
    {
        if (param == "UPGRADES")
        {
            ToUpgradeScreen();
        }
    }
}
