using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButtonController : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject settingScreen;

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

    public void OnMainMenu(LevelLoader item)
    {
        item.LoadLevel("StartMenu");
    }

    public void OnSettings()
    {
        pauseScreen.SetActive(false);
        settingScreen.SetActive(true);
    }

    public void OnBack()
    {
        pauseScreen.SetActive(false);
    }
}
