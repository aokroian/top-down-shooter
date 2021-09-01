using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseScreen : MonoBehaviour
{

    public UIDocument pauseDoc;
    public ProgressionHolder progressionHolder;
    public ScoreCounter scoreCounter;
    public IngameProgressionManager progressionManager;
    public AudioSource audioSource;

    private VisualElement rootEl;
    private VisualElement expContainer;
    private Label expLabelEl;
    private Button resumeEl;
    private Button settingsEl;
    private Button newRunEl;
    private Button mainMenuEl;

    private Action settingsAction;
    private Action gameAction;
    private Action newRunAction;
    private Action mainMenuAction;

    private Label scoreLabel;
    private Label topScoreLabel;

    private bool dead;

    private bool animationPlayed;

    void OnEnable()
    {
        rootEl = pauseDoc.rootVisualElement;
        expContainer = rootEl.Q("ExpContainer");
        expLabelEl = expContainer.Q<Label>("ExpLabel");
        resumeEl = rootEl.Q<Button>("Resume");
        settingsEl = rootEl.Q<Button>("Settings");
        newRunEl = rootEl.Q<Button>("NewRun");
        mainMenuEl = rootEl.Q<Button>("MainMenu");

        resumeEl.RegisterCallback<ClickEvent>(e => gameAction?.Invoke());
        settingsEl.RegisterCallback<ClickEvent>(e => settingsAction?.Invoke());
        newRunEl.RegisterCallback<ClickEvent>(e => newRunAction?.Invoke());
        mainMenuEl.RegisterCallback<ClickEvent>(e => mainMenuAction?.Invoke());

        scoreLabel = rootEl.Q<Label>("ScoreLabel");
        topScoreLabel = rootEl.Q<Label>("TopScoreLabel");
        ChangeDeathScreen();
        SetScore();
    }

    public void SetDead(bool dead)
    {
        this.dead = dead;
        ChangeDeathScreen();
    }

    private void ChangeDeathScreen()
    {
        if (expContainer != null) {
            expContainer.style.display = dead ? DisplayStyle.Flex : DisplayStyle.None;
            resumeEl.style.display = dead ? DisplayStyle.None : DisplayStyle.Flex;
            settingsEl.style.display = dead ? DisplayStyle.None : DisplayStyle.Flex;

            if (animationPlayed)
            {
                expLabelEl.text = progressionHolder.exp.ToString();
            } else
            {
                expLabelEl.text = progressionManager.startExp.ToString();
            }

            if (dead && !animationPlayed && this.enabled)
            {
                Debug.Log("Animation played");
                StartExpAnimation();
            }
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

            float deltaTime = 0.03f + Mathf.Min(40f / (float) expLeft, 0.2f);
            yield return new WaitForSecondsRealtime(deltaTime);
        }
    }

    public void SetSettingsAction(Action settingsAction)
    {
        this.settingsAction = settingsAction;
    }

    public void SetGameAction(Action gameAction)
    {
        this.gameAction = gameAction;
    }

    public void SetNewRunAction(Action newRunAction)
    {
        this.newRunAction = newRunAction;
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
        topScoreLabel.text = progressionHolder.topScore.ToString();
    }
}
