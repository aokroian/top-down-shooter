using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.0f;

    public void LoadLevel(string levelName)
    {
        StartCoroutine(StartLevel(levelName));
    }

    IEnumerator StartLevel(string levelName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelName);
    }
}
