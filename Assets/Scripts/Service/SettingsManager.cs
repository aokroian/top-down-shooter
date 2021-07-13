using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SettingsManager : SettingsSaver
{

    void Start()
    {
        LoadSettings();
        ApplySettings();
    }

    public void LoadSettings()
    {
        settings.vibration = GetBool(VIBRATION, true);
        settings.music = GetBool(MUSIC, true);
        settings.sfx = GetBool(SFX, true);
        settings.graphics = PlayerPrefs.GetString(GRAPHICS, QualitySettings.names[QualitySettings.GetQualityLevel()]);
        settings.fps = GetBool(FPS, false);
        settings.language = PlayerPrefs.GetString(LANGUAGE, LocalizationSettings.SelectedLocale.LocaleName);
    }
}
