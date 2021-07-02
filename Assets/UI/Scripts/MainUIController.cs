using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIController : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject settingsScreen;
    public GameObject upgradeScreen;
    

    public void ToTitleScreen()
    {
        titleScreen.SetActive(true);
        settingsScreen.SetActive(false);
        upgradeScreen.SetActive(false);
    }

    public void ToSettingsScreen()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(true);
        upgradeScreen.SetActive(false);
    }

    public void ToUpgradeScreen()
    {
        titleScreen.SetActive(false);
        settingsScreen.SetActive(false);
        upgradeScreen.SetActive(true);
    }
}
