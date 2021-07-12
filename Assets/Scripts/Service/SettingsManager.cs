using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : SettingsSaver
{

    // Start is called before the first frame update
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
    }
}
