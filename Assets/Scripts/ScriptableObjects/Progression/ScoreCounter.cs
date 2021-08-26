using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreCounter : MonoBehaviour
{
    public ProgressionHolder progressionHolder;
    public GameEvent progressionSaveEvent;
    public UIDocument scoreDocument;
    public GameObject player;

    public float enemyCostMultiplier = 10f;
    public float distanceMultiplier = 1f;

    public int currentScore { get; private set; }
    private int maxDistanceInt;

    private Label scoreLabel;

    private void OnEnable()
    {
        scoreLabel = scoreDocument.rootVisualElement.Q<Label>();
        scoreLabel.text = "0";
    }

    void Update()
    {
        int distance = (int) Vector3.Distance(Vector3.zero, player.transform.position);
        if (distance > maxDistanceInt)
        {
            currentScore += Mathf.CeilToInt((distance - maxDistanceInt) * distanceMultiplier);
            maxDistanceInt = distance;
            updateScoreLabel();
        }
    }

    private void OnApplicationQuit()
    {
        WriteScoreToStore();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            WriteScoreToStore();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            WriteScoreToStore();
        }
    }

    private void OnDestroy()
    {
        WriteScoreToStore();
    }

    public void EnemyToScore(EnemyEventParam param)
    {
        currentScore += Mathf.CeilToInt(param.cost * enemyCostMultiplier);
        updateScoreLabel();
    }

    public void WriteScoreToStore()
    {
        if (currentScore >= progressionHolder.topScore)
        {
            progressionHolder.topScore = currentScore;
            progressionSaveEvent.Raise();
        }
    }

    private void updateScoreLabel()
    {
        scoreLabel.text = currentScore.ToString();
    }
}
