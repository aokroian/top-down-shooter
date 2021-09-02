using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements.InputSystem;

public class GameUIController : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject settingsScreen;
    public GameObject deathScreen;
    public ChangeSceneEvent changeSceneEvent;
    public GameLoopController gameLoopController;

    public InputSystemUIInputModule oldEventSystem;
    public InputSystemEventSystem newEventSystem;

    private PauseScreen pauseController;
    private DeathScreen deathController;
    
    void Start()
    {
        pauseController = pauseScreen.GetComponent<PauseScreen>();
        pauseController.SetGameAction(Resume);
        pauseController.SetSettingsAction(ToSettingsScreen);
        pauseController.SetNewRunAction(NewRun);
        pauseController.SetMainMenuAction(ToMainMenu);

        settingsScreen.GetComponent<SettingsScreen>().SetBackAction(() => ToPauseScreen());

        deathController = deathScreen.GetComponent<DeathScreen>();
        deathController.SetNewRunAction(NewRun);
        deathController.SetMainMenuAction(ToMainMenu);

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

        oldEventSystem.enabled = true;
        //newEventSystem.enabled = false;
    }

    private void ToPauseScreen()
    {
        pauseScreen.SetActive(true);
        settingsScreen.SetActive(false);
        deathScreen.SetActive(false);

        oldEventSystem.enabled = false;
        newEventSystem.enabled = true;
    }

    private void ToSettingsScreen()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(true);
        deathScreen.SetActive(false);
    }

    private void ToDeathScreen()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        deathScreen.SetActive(true);

        oldEventSystem.enabled = false;
        newEventSystem.enabled = true;
    }

    private void NewRun()
    {
        //levelLoader.LoadLevel("Main");
        //gameLoopController.UnPause();
        var param = new ChangeSceneEventParam(SceneEnum.GAME, SceneEnum.GAME, false);
        changeSceneEvent.Raise(param);
    }

    private void ToMainMenu()
    {
        //levelLoader.LoadLevel("StartMenu");
        var param = new ChangeSceneEventParam(SceneEnum.TITLE, SceneEnum.GAME, false);
        changeSceneEvent.Raise(param);
    }

    private void OnStateChange(GameLoopController.GameState state)
    {
        if (state == GameLoopController.GameState.RUNNING)
        {
            HideScreens();
        } else if (state == GameLoopController.GameState.PAUSE)
        {
            ToPauseScreen();
        } else if (state == GameLoopController.GameState.DEAD)
        {
            ToDeathScreen();
        }
    }
}
