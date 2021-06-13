using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnApplicationPause(bool pause)
    {
        
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        paused = true;
        ShowMenu();
    }

    private void UnPause()
    {
        HideMenu();
        Time.timeScale = 1f;
        paused = false;
    }

    private void ShowMenu()
    {
        // TODO: Vova sdelay menu
        // if currentState == GameState.DEAD, dead version menu
    }

    private void HideMenu()
    {
        // TODO: Vova spryach menu
    }
}
