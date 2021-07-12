using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour
{

    //public MainUIController manager;
    public SettingsSaver settingsSaver;
    public SettingsHolder settings;

    private VisualElement rootEl;
    private ToggleVisualElement vibrationEl;
    private ToggleVisualElement musicEl;
    private ToggleVisualElement sfxEl;
    private SelectorVisualElement graphicsEl;
    private ToggleVisualElement fpsEl;

    private Action backAction;

    private void OnEnable()
    {
        rootEl = GetComponent<UIDocument>().rootVisualElement;
        vibrationEl = rootEl.Q<TemplateContainer>("Vibration").Q<ToggleVisualElement>();
        musicEl = rootEl.Q<TemplateContainer>("Music").Q<ToggleVisualElement>();
        sfxEl = rootEl.Q<TemplateContainer>("Sfx").Q<ToggleVisualElement>();
        graphicsEl = rootEl.Q<TemplateContainer>("Graphics").Q<SelectorVisualElement>();
        fpsEl = rootEl.Q<TemplateContainer>("Fps").Q<ToggleVisualElement>();

        rootEl.Q("BackToTitle").RegisterCallback<ClickEvent>(e => backAction?.Invoke());
        fpsEl.SetLabels("30", "60");
        graphicsEl.SetValues(new List<string>(QualitySettings.names));

        CreateHandlers();
        Restore();
    }

    private void CreateHandlers()
    {
        vibrationEl.SetToggleCallback(v =>
        {
            settings.vibration = v;
            settingsSaver.ApplyAndSave();
        });

        musicEl.SetToggleCallback(v =>
        {
            settings.music = v;
            settingsSaver.ApplyAndSave();
        });

        sfxEl.SetToggleCallback(v =>
        {
            settings.sfx = v;
            settingsSaver.ApplyAndSave();
        });

        graphicsEl.SetChangeCallback(v =>
        {
            settings.graphics = v;
            settingsSaver.ApplyAndSave();
        });

        fpsEl.SetToggleCallback(v =>
        {
            settings.fps = v;
            settingsSaver.ApplyAndSave();
        });
    }

    private void Restore()
    {
        vibrationEl.SetValue(settings.vibration);
        musicEl.SetValue(settings.music);
        sfxEl.SetValue(settings.sfx);
        graphicsEl.SetSelected(settings.graphics);
        fpsEl.SetValue(settings.fps);
    }

    public void SetBackAction(Action backAction)
    {
        this.backAction = backAction;
    }
}
