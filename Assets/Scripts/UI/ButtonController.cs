using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ButtonController : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject settingScreen;
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;

    public void Start()
    {
        List<string> resolutionList = Screen.resolutions
            .ToList()
            .Select(e => e.width + "x" + e.height)
            .ToList();

        resolutionDropdown.AddOptions(resolutionList);

        resolutionDropdown.value = Screen.resolutions
            .ToList()
            .FindIndex(resolution => resolution.Equals(Screen.currentResolution));
        resolutionDropdown.RefreshShownValue();
    }

    public void OnMenuItemEnter(GameObject item)
    {
        item.GetComponent<Text>().color = Color.yellow;
    }

    public void OnMenuItemExit(GameObject item)
    {
        item.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f);
    }

    public void OnNewGame(LevelLoader item)
    {
        item.LoadLevel("Main");
    }

    public void OnResume(LevelLoader item)
    {
        item.LoadLevel("Main");
    }

    public void OnSettings()
    {
        mainMenuScreen.SetActive(false);
        settingScreen.SetActive(true);
    }

    public void SetResolutionLevel(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OnFullScreenValueChange(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetQuality(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void SetVolume(float volumeLevel)
    {
        audioMixer.SetFloat("volume", volumeLevel);
    }

    public void OnSettingsBack()
    {
        mainMenuScreen.SetActive(true);
        settingScreen.SetActive(false);
    }
}