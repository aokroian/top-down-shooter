using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractUpgrade : ScriptableObject
{
    [HideInInspector]
    public UpgradeType upgradeType;

    public bool isRoot;
    public AbstractUpgrade[] children;
    public Texture2D image;
    public int cost;
    public string upgradeName;
    public string description;
}
