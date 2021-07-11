using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour
{

    public MainUIController manager;

    private VisualElement rootEl;
    private ToggleVisualElement vibrationEl;
    private ToggleVisualElement musicEl;
    private ToggleVisualElement sfxEl;
    private SelectorVisualElement graphicsEl;
    private ToggleVisualElement fpsEl;

    private void OnEnable()
    {
        rootEl = GetComponent<UIDocument>().rootVisualElement;
        vibrationEl = rootEl.Q<TemplateContainer>("Vibration").Q<ToggleVisualElement>();
        musicEl = rootEl.Q<TemplateContainer>("Music").Q<ToggleVisualElement>();
        sfxEl = rootEl.Q<TemplateContainer>("Sfx").Q<ToggleVisualElement>();
        graphicsEl = rootEl.Q<TemplateContainer>("Graphics").Q<SelectorVisualElement>();
        fpsEl = rootEl.Q<TemplateContainer>("Fps").Q<ToggleVisualElement>();

        rootEl.Q("BackToTitle").RegisterCallback<ClickEvent>(e => manager.ToTitleScreen());
        fpsEl.SetLabels("30", "60");
        graphicsEl.SetValues(new List<string>(QualitySettings.names));
        
    }
}
