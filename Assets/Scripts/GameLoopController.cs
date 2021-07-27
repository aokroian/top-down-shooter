using System;
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
    //public GameObject pauseScreen;

    private HashSet<Action<GameState>> stateChangeHandlers = new HashSet<Action<GameState>>();

    // Update is called once per frame
    void Update()
    {
        if (playerTarget.health == 0f)
        {
            Pause(GameState.DEAD);
        }
    }

    private void Start()
    {
        UnPause();
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

    public void Pause(GameState state = GameState.PAUSE)
    {
        Time.timeScale = 0f;
        paused = true;
        currentState = state;
        //ShowMenu();
        NotifyAll();
    }

    public void UnPause()
    {
        //HideMenu();
        Time.timeScale = 1f;
        paused = false;
        currentState = GameState.RUNNING;
        NotifyAll();
    }

    public GameState GetGameState()
    {
        return currentState;
    }

    public void AddStateChangeHandler(Action<GameState> handler)
    {
        stateChangeHandlers.Add(handler);
    }

    public void RemoveStateChangeHandler(Action<GameState> handler)
    {
        stateChangeHandlers.Remove(handler);
    }

    public void NotifyAll()
    {
        foreach(Action<GameState> handler in stateChangeHandlers)
        {
            handler(currentState);
        }
    }
}
