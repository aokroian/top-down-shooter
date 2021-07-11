using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private const string VIBRATION = "vibration";
    private const string MUSIC = "music";
    private const string SFX = "sfx";
    private const string GRAPHICS = "graphics";
    private const string FPS = "fps";


    public SettingsHolder settings;

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

    private void ApplySettings()
    {
        // graphics
        int index = new List<string>(QualitySettings.names).IndexOf(settings.graphics);
        QualitySettings.SetQualityLevel(index);

        // fps
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = settings.fps ? 60 : 30;
    }

    private bool GetBool(string key, bool defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    private void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
}
