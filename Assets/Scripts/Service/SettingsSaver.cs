using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSaver : MonoBehaviour
{
    protected const string VIBRATION = "vibration";
    protected const string MUSIC = "music";
    protected const string SFX = "sfx";
    protected const string GRAPHICS = "graphics";
    protected const string FPS = "fps";

    public SettingsHolder settings;

    public void ApplyAndSave()
    {
        ApplySettings();
        SetBool(VIBRATION, settings.vibration);
        SetBool(MUSIC, settings.music);
        SetBool(SFX, settings.sfx);
        PlayerPrefs.SetString(GRAPHICS, settings.graphics);
        SetBool(FPS, settings.fps);
        PlayerPrefs.Save();
    }

    protected void ApplySettings()
    {
        // graphics
        int index = new List<string>(QualitySettings.names).IndexOf(settings.graphics);
        QualitySettings.SetQualityLevel(index);

        // fps
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = settings.fps ? 60 : 30;
    }

    protected bool GetBool(string key, bool defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    protected void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
}
