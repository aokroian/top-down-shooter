using UnityEngine;

public abstract class AbstractUpgrade : ScriptableObject
{
    [HideInInspector]
    public UpgradeType upgradeType;

    public bool isRoot;
    public AbstractUpgrade[] children;
    public Sprite image;
    public int cost;
    public string upgradeName;
    public string description;
}
