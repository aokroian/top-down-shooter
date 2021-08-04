using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Audio;

public class SettingsSaver : MonoBehaviour
{
    protected const string VIBRATION = "vibration";
    protected const string MUSIC = "music";
    protected const string SFX = "sfx";
    protected const string GRAPHICS = "graphics";
    protected const string FPS = "fps";
    protected const string LANGUAGE = "language";

    public SettingsHolder settings;

    // vars for audio
    public AudioMixer audioMixer;
    public string masterVolumeParamName;
    public string musicVolumeParamName;
    public string sfxVolumeParamName;

    public void ApplyAndSave()
    {
        ApplySettings();
        SetBool(VIBRATION, settings.vibration);
        SetBool(MUSIC, settings.music);
        SetBool(SFX, settings.sfx);
        if (!settings.music)
        {
            audioMixer.SetFloat(musicVolumeParamName, -80);
        }
        else if (settings.music)
        {
            audioMixer.SetFloat(musicVolumeParamName, 0);
        }
        if (!settings.sfx)
        {
            audioMixer.SetFloat(sfxVolumeParamName, -80);
        }
        else if (settings.sfx)
        {
            audioMixer.SetFloat(sfxVolumeParamName, 0);
        }

        PlayerPrefs.SetString(GRAPHICS, settings.graphics);
        SetBool(FPS, settings.fps);
        PlayerPrefs.SetString(LANGUAGE, settings.language);
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
        Debug.Log("vsyncCount: " + QualitySettings.vSyncCount);

        // language
        var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(v => v.LocaleName == settings.language);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
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
