using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TitleScreen : MonoBehaviour
{
    public MainUIController manager;
    public ProgressionHolder progressionHolder;


    private VisualElement rootEl;

    private void OnEnable()
    {
        rootEl = GetComponent<UIDocument>().rootVisualElement;
        rootEl.Q("NewRunButton").RegisterCallback<ClickEvent>(e => manager.ToUpgradeScreen());
        rootEl.Q("Settings").RegisterCallback<ClickEvent>(e => manager.ToSettingsScreen());
        rootEl.Q("Credits").RegisterCallback<ClickEvent>(e => manager.ToCreditsScreen());
        ShowTopScore();
        ShowCredits();
        ShowProgressionPoints();
    }

    public void ShowTopScore()
    {
        rootEl.Q<Label>("TopScoreLabel").text = progressionHolder.topScore.ToString();
    }

    public void ShowCredits()
    {
        rootEl.Q<Label>("CreditsLabel").text = progressionHolder.moneyCount.ToString();
    }

    public void ShowProgressionPoints()
    {
        rootEl.Q<Label>("ProgressionPointsLabel").text = progressionHolder.exp.ToString();
    }
}
