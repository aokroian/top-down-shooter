using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SettingsManager : SettingsSaver, Loadable
{
    public int _loadCost = 3;
    public int loadCost => _loadCost;

    public void LoadSettings()
    {
        settings.vibration = GetBool(VIBRATION, true);
        settings.music = GetBool(MUSIC, true);
        settings.sfx = GetBool(SFX, true);
        settings.graphics = PlayerPrefs.GetString(GRAPHICS, QualitySettings.names[QualitySettings.GetQualityLevel()]);
        settings.fps = GetBool(FPS, false);
        settings.language = PlayerPrefs.GetString(LANGUAGE, LocalizationSettings.SelectedLocale.LocaleName);
    }

    public void Load(Action onLoad)
    {
        LoadSettings();
        ApplySettings();
        onLoad();
    }
}
