using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject settingsScreen;
    public LevelLoader levelLoader;
    public GameLoopController gameLoopController;

    private PauseScreen pauseController;
    
    void Start()
    {
        pauseController = pauseScreen.GetComponent<PauseScreen>();
        pauseController.SetGameAction(Resume);
        pauseController.SetSettingsAction(ToSettingsScreen);
        pauseController.SetNewRunAction(NewRun);
        pauseController.SetMainMenuAction(ToMainMenu);

        settingsScreen.GetComponent<SettingsScreen>().SetBackAction(() => ToPauseScreen(false));

        gameLoopController.AddStateChangeHandler(OnStateChange);
    }

    private void Resume()
    {
        gameLoopController.UnPause();
    }

    private void HideScreens()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }

    private void ToPauseScreen(bool dead)
    {
        pauseScreen.SetActive(true);
        pauseController.SetDead(dead);
        settingsScreen.SetActive(false);
    }

    private void ToSettingsScreen()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    private void NewRun()
    {
        levelLoader.LoadLevel("Main");
    }

    private void ToMainMenu()
    {
        levelLoader.LoadLevel("StartMenu");
    }

    private void OnStateChange(GameLoopController.GameState state)
    {
        if (state == GameLoopController.GameState.RUNNING)
        {
            HideScreens();
        } else if (state == GameLoopController.GameState.PAUSE)
        {
            ToPauseScreen(false);
        } else if (state == GameLoopController.GameState.DEAD)
        {
            ToPauseScreen(true);
        }
    }
}
