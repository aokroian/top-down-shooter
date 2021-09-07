using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ProgressionHolder", order = 55)]
public class ProgressionHolder : ScriptableObject
{
    public int moneyCount;
    public int topScore;
    public int exp;

    public AbstractUpgrade[] allUpgrades;
    private HashSet<AbstractUpgrade> purchasedUpgrades = new HashSet<AbstractUpgrade>();
    private HashSet<WeaponUnlock> selectedUpgrades = new HashSet<WeaponUnlock>(); // Or should be in SO?

    public PlayerUpgrade[] allPlayerUpgrades;
    public HashSet<PlayerUpgrade> purchasedPlayerUpgrades = new HashSet<PlayerUpgrade>();

    public ProgressionHolder()
    {

    }

    public void ClearAll()
    {
        moneyCount = 0;
        topScore = 0;
        exp = 0;
        allUpgrades = null;
        purchasedUpgrades.Clear();
        selectedUpgrades.Clear();
        allPlayerUpgrades = null;
        purchasedPlayerUpgrades.Clear();

        OnEnable();
    }

    private void OnEnable()
    {
        //Debug.Log("ProgressionHolder OnEnable!!!!!");
        allUpgrades = Resources.LoadAll<AbstractUpgrade>("UpgradesSO/Weapons");
        allPlayerUpgrades = Resources.LoadAll<PlayerUpgrade>("UpgradesSO/Player");
        CalcTiers();
    }

    private void CalcTiers()
    {
        foreach (AbstractUpgrade root in allUpgrades.Where(e => e.isRoot))
        {
            if (root.upgradeType == UpgradeType.WEAPON_UPGRADE)
            {
                SetTierRecursive((WeaponUpgrade) root, 1);
            }
            else
            {
                //Debug.Log(root.name);
                foreach (WeaponUpgrade child in root.children.Where(e => e.upgradeType == UpgradeType.WEAPON_UPGRADE))
                {
                    SetTierRecursive(child, 1);
                }
            }
        }
    }

    private void SetTierRecursive(WeaponUpgrade upgrade, int currentTier)
    {
        upgrade.tier = currentTier;
        if (upgrade.children.Length != 0)
        {
            foreach(WeaponUpgrade child in upgrade.children)
            {
                SetTierRecursive(child, currentTier + 1);
            }
        }
    }

    public void SetPurchasedUpgrades(int[] purchased)
    {
        AddUpgradesToSet(purchased, purchasedUpgrades);
    }

    public void SetSelectedUpgrades(int[] selected)
    {
        AddUpgradesToSet(selected, selectedUpgrades);
    }

    private void AddUpgradesToSet<T>(int[] ids, HashSet<T> set) where T:AbstractUpgrade
    {
        foreach (int id in ids)
        {
            var item = allUpgrades.First(v => v.name.GetHashCode() == id);
            if (item != null)
            {
                set.Add((T) item);
            }
        }
    }

    public void AddPurchasedUpgrade(AbstractUpgrade upgrade)
    {
        purchasedUpgrades.Add(upgrade);
    }

    public AbstractUpgrade[] GetAllUpgrades()
    {
        return allUpgrades;
    }

    public HashSet<AbstractUpgrade> GetPurchasedUpgrades()
    {
        return purchasedUpgrades;
    }

    public int[] GetPurchasedUpgradesId()
    {
        return GetIdsFromSet(purchasedUpgrades);
    }

    public bool AddSelectedByUpgrade(AbstractUpgrade upgrade)
    {
        foreach (WeaponUnlock root in allUpgrades.Where(e => e.upgradeType == UpgradeType.WEAPON_UNLOCK))
        {
            AbstractUpgrade cur = root;
            while (cur != null)
            {
                if (cur == upgrade)
                {
                    selectedUpgrades.Add(root);
                    return true;
                }
                cur = cur.children.Length == 0 ? null : cur.children[0];
            }
        }
        return false;
    }

    public bool RemoveSelectedByUpgrade(AbstractUpgrade upgrade)
    {
        foreach (WeaponUnlock root in selectedUpgrades)
        {
            AbstractUpgrade cur = root;
            while (cur != null)
            {
                if (cur == upgrade)
                {
                    selectedUpgrades.Remove(root);
                    return true;
                }
                cur = cur.children.Length == 0 ? null : cur.children[0];
            }
        }
        return false;
    }

    public HashSet<WeaponUnlock> GetSelected()
    {
        return selectedUpgrades;
    }

    public int[] GetSelectedIds()
    {
        return GetIdsFromSet(selectedUpgrades);
    }

    private int[] GetIdsFromSet<T>(HashSet<T> set) where T : AbstractUpgrade
    {
        List<int> ids = new List<int>();
        foreach (AbstractUpgrade upgrade in set)
        {
            ids.Add(upgrade.name.GetHashCode());
        }
        return ids.ToArray();
    }

    public bool IsPurchased(AbstractUpgrade upgrade)
    {
        return purchasedUpgrades.FirstOrDefault(v => v == upgrade) != null;
    }

    public bool IsSelectable(AbstractUpgrade upgrade)
    {
        foreach (AbstractUpgrade root in allUpgrades.Where(e => e.upgradeType == UpgradeType.WEAPON_UNLOCK))
        {
            var cur = root;
            while (cur != null)
            {
                if (cur == upgrade)
                {
                    return true;
                }
                cur = cur.children.Length == 0 ? null : cur.children[0];
            }
        }
        return false;
    }

    public bool IsSelectedRoot(AbstractUpgrade upgrade)
    {
        
        foreach (WeaponUnlock root in selectedUpgrades)
        {
            AbstractUpgrade cur = root;
            while (cur != null)
            {
                if (cur == upgrade)
                {
                    return true;
                }
                cur = cur.children.Length == 0 ? null : cur.children[0];
            }
        }
        return false;
    }



    public void SetPurchasedPlayerUpgrades(int[] purchased)
    {
        AddPlayerUpgradesToSet(purchased, purchasedPlayerUpgrades);
    }

    private void AddPlayerUpgradesToSet<T>(int[] ids, HashSet<T> set) where T : PlayerUpgrade
    {
        foreach (int id in ids)
        {
            var item = allPlayerUpgrades.First(v => v.name.GetHashCode() == id);
            if (item != null)
            {
                set.Add((T)item);
            }
        }
    }

    public void AddPurchasedPlayerUpgrade(PlayerUpgrade upgrade)
    {
        purchasedPlayerUpgrades.Add(upgrade);
    }

    public PlayerUpgrade[] GetAllPlayerUpgrades()
    {
        return allPlayerUpgrades;
    }

    public HashSet<PlayerUpgrade> GetPurchasedPlayerUpgrades()
    {
        return purchasedPlayerUpgrades;
    }

    public int[] GetPurchasedPlayerUpgradesId()
    {
        return GetIdsFromSet(purchasedPlayerUpgrades);
    }
}
