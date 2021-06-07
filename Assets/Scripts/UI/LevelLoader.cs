using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.0f;
    public GameObject background;
    public bool isStartMenuScreen = false;

    public void Start()
    {
        if (!isStartMenuScreen)
        {
            background.SetActive(true);
        }
    }

    public void LoadLevel(string levelName)
    {
        background.SetActive(true);
        StartCoroutine(StartLevel(levelName));
    }

    IEnumerator StartLevel(string levelName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelName);
    }
}
