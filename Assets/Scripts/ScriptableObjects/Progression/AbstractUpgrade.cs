using UnityEngine;

public abstract class AbstractUpgrade : ScriptableObject
{
    [HideInInspector]
    public UpgradeType upgradeType;
    //[HideInInspector]
    //public bool purchased;

    public bool isRoot;
    public AbstractUpgrade[] children;
    public Texture2D image;
    public int cost;
    public string upgradeName;
    public string description;
    public int orderInUpgradeScreen;
}
