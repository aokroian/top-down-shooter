using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseScreen : MonoBehaviour
{

    public UIDocument pauseDoc;

    private VisualElement rootEl;
    private Button resumeEl;
    private Button settingsEl;
    private Button newRunEl;
    private Button mainMenuEl;

    private Action settingsAction;
    private Action gameAction;
    private Action newRunAction;
    private Action mainMenuAction;
    
    void OnEnable()
    {
        rootEl = pauseDoc.rootVisualElement;
        resumeEl = rootEl.Q<Button>("Resume");
        settingsEl = rootEl.Q<Button>("Settings");
        newRunEl = rootEl.Q<Button>("NewRun");
        mainMenuEl = rootEl.Q<Button>("MainMenu");

        resumeEl.RegisterCallback<ClickEvent>(e => gameAction?.Invoke());
        settingsEl.RegisterCallback<ClickEvent>(e => settingsAction?.Invoke());
        newRunEl.RegisterCallback<ClickEvent>(e => newRunAction?.Invoke());
        mainMenuEl.RegisterCallback<ClickEvent>(e => mainMenuAction?.Invoke());
    }

    public void SetDead(bool dead)
    {
        resumeEl.style.display = dead ? DisplayStyle.None : DisplayStyle.Flex;
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
}
