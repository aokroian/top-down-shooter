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
        rootEl.Q("NewRun").RegisterCallback<ClickEvent>(e => manager.ToUpgradeScreen());
        rootEl.Q("Settings").RegisterCallback<ClickEvent>(e => manager.ToSettingsScreen());

        rootEl.Q<Label>("TopScoreLabel").text = progressionHolder.topScore.ToString();
    }
}
