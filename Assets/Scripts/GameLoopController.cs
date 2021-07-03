using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoopController : MonoBehaviour
{
    public enum GameState
    {
        RUNNING,
        PAUSE,
        DEAD
    }

    public static bool paused = false;

    public GameState currentState = GameState.RUNNING;
    public Target playerTarget;
    public GameObject pauseScreen;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTarget.health == 0f)
        {
            currentState = GameState.DEAD;
            Pause();
        }
    }

    // pause on focus is needed only in release version

    //private void OnApplicationFocus(bool focus)
    //{
    //    if (!focus)
    //    {
    //        currentState = GameState.PAUSE;
    //        Pause();
    //    }
    //}

    public void Pause()
    {
        Time.timeScale = 0f;
        paused = true;
        ShowMenu();
    }

    public void UnPause()
    {
        HideMenu();
        Time.timeScale = 1f;
        paused = false;
    }

    public GameState GetGameState()
    {
        return currentState;
    }

    private void ShowMenu()
    {
        pauseScreen.SetActive(true);
    }

    private void HideMenu()
    {
        pauseScreen.SetActive(false);
    }
}
