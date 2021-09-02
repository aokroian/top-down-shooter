using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeathScreen : MonoBehaviour
{

    public UIDocument pauseDoc;
    public ProgressionHolder progressionHolder;
    public ScoreCounter scoreCounter;
    public IngameProgressionManager progressionManager;
    public AudioSource audioSource;

    private VisualElement rootEl;
    private VisualElement expContainer;
    private Label expLabelEl;
    private Button newRunEl;
    private Button upgradesEl;
    private Button mainMenuEl;

    private Action newRunAction;
    private Action upgradesAction;
    private Action mainMenuAction;

    private Label scoreLabel;

    private bool animationPlayed;

    void OnEnable()
    {
        rootEl = pauseDoc.rootVisualElement;
        expContainer = rootEl.Q("ExpContainer");
        expLabelEl = expContainer.Q<Label>("ExpLabel");
        newRunEl = rootEl.Q<Button>("NewRun");
        upgradesEl = rootEl.Q<Button>("Upgrades");
        mainMenuEl = rootEl.Q<Button>("MainMenu");

        newRunEl.RegisterCallback<ClickEvent>(e => newRunAction?.Invoke());
        upgradesEl.RegisterCallback<ClickEvent>(e => upgradesAction?.Invoke());
        mainMenuEl.RegisterCallback<ClickEvent>(e => mainMenuAction?.Invoke());

        scoreLabel = rootEl.Q<Label>("ScoreLabel");
        SetScore();
        ChangeDeathScreen();
    }

    private void ChangeDeathScreen()
    {
        if (animationPlayed)
        {
            expLabelEl.text = progressionHolder.exp.ToString();
        }
        else
        {
            expLabelEl.text = progressionManager.startExp.ToString();
        }

        if (!animationPlayed && this.enabled)
        {
            StartExpAnimation();
        }
    }

    private void StartExpAnimation()
    {
        animationPlayed = true;
        StartCoroutine(ExpAnimationCoroutine());
    }

    private IEnumerator ExpAnimationCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        int expLeft = scoreCounter.currentScore;
        int expCurrent = progressionManager.startExp;
        while (expLeft > 0)
        {
            int expDelta = Mathf.Min(100 + 100 * Mathf.FloorToInt(expLeft / 10000), expLeft);
            expLeft -= expDelta;
            expCurrent += expDelta;
            scoreLabel.text = expLeft.ToString();
            expLabelEl.text = expCurrent.ToString();
            audioSource.Play();

            float deltaTime = 0.03f + Mathf.Min(40f / (float)expLeft, 0.2f);
            yield return new WaitForSecondsRealtime(deltaTime);
        }
    }

    public void SetNewRunAction(Action newRunAction)
    {
        this.newRunAction = newRunAction;
    }

    public void SetUpgradesAction(Action upgradesAction)
    {
        this.upgradesAction = upgradesAction;
    }

    public void SetMainMenuAction(Action mainMenuAction)
    {
        this.mainMenuAction = mainMenuAction;
    }

    private void SetScore()
    {
        if (animationPlayed)
        {
            scoreLabel.text = "0";
            return;
        }
        scoreLabel.text = scoreCounter.currentScore.ToString();
        if (scoreCounter.currentScore > progressionHolder.topScore)
        {
            progressionHolder.topScore = scoreCounter.currentScore;
        }

        if (scoreCounter.currentScore > progressionManager.startTopScore) {
            rootEl.Q<Label>("RecordLabel").style.display = DisplayStyle.Flex;
        } else
        {
            rootEl.Q<Label>("RecordLabel").style.display = DisplayStyle.None;
        }
    }
}
