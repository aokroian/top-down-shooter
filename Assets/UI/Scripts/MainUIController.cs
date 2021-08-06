using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    private const string GAME_SCENE_NAME = "Main";

    public GameObject titleScreen;
    public GameObject settingsScreen;
    public GameObject upgradeScreen;

    public LocalizationTableHolder localizationTableHolder;

    public ProgressionManager progressionManager;

    private void Start()
    {
        localizationTableHolder.Init();
        settingsScreen.GetComponent<SettingsScreen>().SetBackAction(ToTitleScreen);

        progressionManager.LoadFromSaveFile();
    }

    public void ToTitleScreen()
    {
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
        SceneManager.LoadSceneAsync(GAME_SCENE_NAME);
    }
}
