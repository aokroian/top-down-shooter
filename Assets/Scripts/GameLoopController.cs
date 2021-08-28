using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
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
    public ScoreCounter scoreCounter;
    public PlayerAmmoController playerAmmoController;

    public LoadProgressSceneEvent loadProgressSceneEvent;

    public MenuToggleEvent menuToggleEvent;

    private HashSet<Action<GameState>> stateChangeHandlers = new HashSet<Action<GameState>>();

    private float startTime;
    private int startCredit;

    private void Start()
    {
        AnalyticsEvent.GameStart();
        var param = new LoadProgressSceneEP(SceneEnum.GAME, true);
        loadProgressSceneEvent.Raise(param);
        UnPause();
        startTime = Time.time;
        startCredit = scoreCounter.progressionHolder.moneyCount;
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

    public void Death()
    {
        Pause(GameState.DEAD);
    }

    public void Pause(GameState state = GameState.PAUSE)
    {
        if (state == GameState.DEAD) {
            try
            {
                GameOverAnalytics();
            } catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        Time.timeScale = 0f;
        paused = true;
        currentState = state;
        //ShowMenu();
        NotifyAll();
    }

    public void UnPause()
    {
        Debug.Log("UnPause!!!");
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
        menuToggleEvent.Raise(new MenuToggleEventParam(paused));
    }

    private void GameOverAnalytics()
    {
        var data = new Dictionary<string, object>();
        data.Add("time", Time.time - startTime);
        data.Add("distance", scoreCounter.maxDistanceInt);
        data.Add("creditByRun", scoreCounter.progressionHolder.moneyCount - startCredit);
        data.Add("credit", scoreCounter.progressionHolder.moneyCount);
        data.Add("score", scoreCounter.currentScore);
        data.Add("topScore", Mathf.Max(scoreCounter.currentScore, scoreCounter.progressionHolder.topScore));
        AnalyticsEvent.GameOver(eventData: data);

        var ammoData = new Dictionary<string, object>();
        foreach (var pair in playerAmmoController.GetAnalyticsValues())
        {
            ammoData[pair.Key] = pair.Value;
        }
        AnalyticsEvent.LevelFail(SceneManager.GetActiveScene().name, ammoData);
    }
}
