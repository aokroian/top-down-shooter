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
        item.LoadLevel("WeaponSelector");
    }

    public void OnSettings()
    {
        mainMenuScreen.SetActive(false);
        settingScreen.SetActive(true);
    }
}