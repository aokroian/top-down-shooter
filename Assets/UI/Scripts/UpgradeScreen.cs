using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UpgradeScreen : MonoBehaviour
{
    public MainUIController manager;
    public VisualTreeAsset upgradeElement;
    public FloatVariable moneyCount;

    private VisualElement rootEl;
    private ScrollView scrollEl;

    private AbstractUpgrade[] upgrades;

    /*
    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        
        
    }
    */

    private void Start()
    {
        upgrades = Resources.LoadAll<AbstractUpgrade>("UpgradesSO");

        rootEl = GetComponent<UIDocument>().rootVisualElement;
        scrollEl = rootEl.Q<ScrollView>("UpgradesScroll");
        rootEl.Q("BackToTitle").RegisterCallback<ClickEvent>(e => manager.ToTitleScreen());

#if UNITY_STANDALONE || UNITY_EDITOR
        scrollEl.horizontalScrollerVisibility = ScrollerVisibility.Auto;
#endif

        FillUpgradeScroll();
    }

    private void FillUpgradeScroll()
    {
        var sortedUIList = CreateListForUI().OrderBy(e => e.isRoot ? 0 : 1);
        Debug.Log("SortedLength " + sortedUIList.Count());
        foreach (AbstractUpgrade upgrade in sortedUIList)
        {
            AddUpgrade(upgrade);
        }
    }

    // TODO: Move to separate class/interface UpgradesUIProvider
    private List<AbstractUpgrade> CreateListForUI()
    {
        List<AbstractUpgrade> result = new List<AbstractUpgrade>();
        foreach(AbstractUpgrade upgrade in upgrades.Where(e => e.isRoot))
        {
            Debug.Log("Root: " + upgrade);
            result.Add(getFirstNotBought(upgrade));
        }

        return result;
    }

    private AbstractUpgrade getFirstNotBought(AbstractUpgrade first) {
        AbstractUpgrade current = first;
        while (current.purchased && current.children.Length > 0)
        {
            current = current.children[0];
        }
        return current;
    }

    private UpgradeVisualElement AddUpgrade(AbstractUpgrade upgrade)
    {
        var tree = upgradeElement.CloneTree();
        var upgradeEl = tree.Q<UpgradeVisualElement>();

        //upgradeEl.SetIcon(upgrade.image);
        upgradeEl.SetName(upgrade.upgradeName);
        upgradeEl.SetCost(upgrade.cost);

        if (upgrade.upgradeType == UpgradeType.WEAPON_UPGRADE) {
            upgradeEl.SetProgressVisible(upgrade.upgradeType == UpgradeType.WEAPON_UPGRADE);
            int currentTier = ((WeaponUpgrade)upgrade).tier;
            int maxTier = GetMaxTier((WeaponUpgrade)upgrade);
            upgradeEl.SetProgressValue(currentTier, maxTier);
        } else
        {
            upgradeEl.SetProgressVisible(false);
        }

        scrollEl.Add(upgradeEl);
        return upgradeEl;
    }

    private int GetMaxTier(WeaponUpgrade upgrade)
    {
        WeaponUpgrade current = upgrade;
        while (current.children.Length > 0)
        {
            current = (WeaponUpgrade) current.children[0];
        }
        return current.tier;
    }
}
