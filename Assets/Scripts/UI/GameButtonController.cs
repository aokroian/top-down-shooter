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
    public GameObject backButton;

    public GameLoopController gameLoopController;

    private Sprite pauseBackground, deadBackground;

    public void Start()
    {
        pauseBackground = Resources.Load<Sprite>("UI/PauseBackground");
        deadBackground = Resources.Load<Sprite>("UI/DeadBackground");
    }

    public void Update()
    {
        if (gameLoopController.GetGameState() == GameLoopController.GameState.DEAD)
        {
            backButton.SetActive(false);
            pauseScreen.GetComponent<Image>().sprite = deadBackground;
        }
        else
        {
            backButton.SetActive(true);
            pauseScreen.GetComponent<Image>().sprite = pauseBackground;
        }
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
        gameLoopController.UnPause();
    }
}
