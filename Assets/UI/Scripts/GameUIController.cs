using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements.InputSystem;

public class GameUIController : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject settingsScreen;
    //public LevelLoader levelLoader;
    public ChangeSceneEvent changeSceneEvent;
    public GameLoopController gameLoopController;

    public InputSystemUIInputModule oldEventSystem;
    public InputSystemEventSystem newEventSystem;

    private PauseScreen pauseController;
    
    void Start()
    {
        pauseController = pauseScreen.GetComponent<PauseScreen>();
        pauseController.SetGameAction(Resume);
        pauseController.SetSettingsAction(ToSettingsScreen);
        pauseController.SetNewRunAction(NewRun);
        pauseController.SetMainMenuAction(ToMainMenu);

        settingsScreen.GetComponent<SettingsScreen>().SetBackAction(() => ToPauseScreen());

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

        oldEventSystem.enabled = false;
        newEventSystem.enabled = true;
    }

    private void ToSettingsScreen()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(true);
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

    private void SetDead(bool dead)
    {
        pauseController.SetDead(dead);
    }

    private void OnStateChange(GameLoopController.GameState state)
    {
        if (state == GameLoopController.GameState.RUNNING)
        {
            HideScreens();
        } else if (state == GameLoopController.GameState.PAUSE)
        {
            ToPauseScreen();
            SetDead(false);
        } else if (state == GameLoopController.GameState.DEAD)
        {
            ToPauseScreen();
            SetDead(true);
        }
    }
}
