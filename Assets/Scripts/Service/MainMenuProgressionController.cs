using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuProgressionController : MonoBehaviour
{
    public ProgressionManager progressionManager;
    public ProgressionHolder progressionHolder;
    
    void Start()
    {
        progressionManager.LoadFromSaveFile();
    }

    public void Save()
    {
        progressionManager.WriteToSaveFile();
    }
}
