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
        Act();
    }

    private void Act()
    {
        switch (currentState)
        {
            case GameState.RUNNING:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    currentState = GameState.PAUSE;
                    Pause();
                } else if (playerTarget.health == 0f)
                {
                    currentState = GameState.DEAD;
                    Pause();
                }
                break;
            case GameState.PAUSE:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    currentState = GameState.RUNNING;
                    UnPause();
                }
                break;
            case GameState.DEAD:

                break;
            default:
                break;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            currentState = GameState.PAUSE;
            Pause();
        }
    }

    private void Pause()
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

    private void ShowMenu()
    {
        pauseScreen.SetActive(true);
    }

    private void HideMenu()
    {
        pauseScreen.SetActive(false);
    }
}