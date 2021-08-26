using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour
{

    //public MainUIController manager;
    public SettingsHolder settings;
    public GameEvent settingsSaveEvent;
    public UIDocumentLocalization localizer;

    private VisualElement rootEl;
    private ToggleVisualElement vibrationEl;
    private ToggleVisualElement musicEl;
    private ToggleVisualElement sfxEl;
    private SelectorVisualElement graphicsEl;
    private ToggleVisualElement fpsEl;
    private SelectorVisualElement languageEl;

    private Action backAction;

    void Awake()
    {
        localizer.SetRebuildCallback(Init);
    }

    private void OnEnable()
    {
        //Init();
    }

    private void Init()
    {
        rootEl = GetComponent<UIDocument>().rootVisualElement;
        vibrationEl = rootEl.Q<TemplateContainer>("Vibration").Q<ToggleVisualElement>();
        musicEl = rootEl.Q<TemplateContainer>("Music").Q<ToggleVisualElement>();
        sfxEl = rootEl.Q<TemplateContainer>("Sfx").Q<ToggleVisualElement>();
        graphicsEl = rootEl.Q<TemplateContainer>("Graphics").Q<SelectorVisualElement>();
        fpsEl = rootEl.Q<TemplateContainer>("Fps").Q<ToggleVisualElement>();
        languageEl = rootEl.Q<TemplateContainer>("Language").Q<SelectorVisualElement>();

        rootEl.Q("BackToTitle").RegisterCallback<ClickEvent>(e => backAction?.Invoke());
        fpsEl.SetLabels("30", "60");
        graphicsEl.SetValues(new List<string>(QualitySettings.names));
        languageEl.SetValues(CreateLocaleStrings()); // Should be in Awake?

        CreateHandlers();
        Restore();
    }

    private List<string> CreateLocaleStrings()
    {
        List<string> result = new List<string>();
        foreach(Locale locale in LocalizationSettings.AvailableLocales.Locales) {
            result.Add(locale.LocaleName);
        }
        return result;
    }

    private void CreateHandlers()
    {
        vibrationEl.SetToggleCallback(v =>
        {
            settings.vibration = v;
            settingsSaveEvent.Raise();
        });

        musicEl.SetToggleCallback(v =>
        {
            settings.music = v;
            settingsSaveEvent.Raise();
        });

        sfxEl.SetToggleCallback(v =>
        {
            settings.sfx = v;
            settingsSaveEvent.Raise();
        });

        graphicsEl.SetChangeCallback(v =>
        {
            settings.graphics = v;
            settingsSaveEvent.Raise();
        });

        fpsEl.SetToggleCallback(v =>
        {
            settings.fps = v;
            settingsSaveEvent.Raise();
        });

        languageEl.SetChangeCallback(v =>
        {
            settings.language = v;
            settingsSaveEvent.Raise();
        });
    }

    private void Restore()
    {
        vibrationEl.SetValue(settings.vibration);
        musicEl.SetValue(settings.music);
        sfxEl.SetValue(settings.sfx);
        graphicsEl.SetSelected(settings.graphics);
        fpsEl.SetValue(settings.fps);
        languageEl.SetSelected(settings.language);
    }

    public void SetBackAction(Action backAction)
    {
        this.backAction = backAction;
    }
}
