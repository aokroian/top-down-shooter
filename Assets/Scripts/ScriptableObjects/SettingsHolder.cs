using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/SettingsHolder", order = 55)]
public class SettingsHolder : ScriptableObject
{
    public bool vibration;
    public bool music;
    public bool sfx;
    public string graphics;
    public bool fps;
    public string language;
}
