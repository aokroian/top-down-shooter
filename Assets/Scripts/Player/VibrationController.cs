using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationController : MonoBehaviour
{
    public SettingsHolder settingsHolder;

    public void Vibrate()
    {
        if (settingsHolder.vibration)
        {
            Vibration.VibratePeek();
        }
    }
}
