using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmoController : MonoBehaviour, IAmmoProvider
{
    public IAmmoConsumer currentWeapon;

    // -1 for endless
    [Header("Max ammo count (-1 for endless)")]
    public int pistolMax = -1;
    public int machinegunMax;
    public int rifleMax;
    public int grenadeMax;

    [Header("Start ammo count")]
    public int pistolDefault = -1;
    public int machinegunDefault;
    public int rifleDefault;
    public int grenadeDefault;

    private Dictionary<AmmoType, int> ammoMap = new Dictionary<AmmoType, int>();
    private Dictionary<AmmoType, int> maxAmmoMap = new Dictionary<AmmoType, int>();

    void Start()
    {
        foreach (AmmoType type in (AmmoType[])System.Enum.GetValues(typeof(AmmoType)))
        {
            switch (type)
            {
                case AmmoType.NONE:
                    maxAmmoMap[type] = 0;
                    ammoMap.Add(type, 0);
                    break;
                case AmmoType.PISTOL:
                    maxAmmoMap[type] = pistolMax;
                    ammoMap.Add(type, maxAmmoMap[type] >= 0 ? Mathf.Min(pistolDefault, pistolMax) : -1);
                    break;
                case AmmoType.MACHINEGUN:
                    maxAmmoMap[type] = machinegunMax;
                    ammoMap.Add(type, maxAmmoMap[type] >= 0 ? Mathf.Min(machinegunDefault, machinegunMax) : -1);
                    break;
                case AmmoType.RIFLE:
                    maxAmmoMap[type] = rifleMax;
                    ammoMap.Add(type, maxAmmoMap[type] >= 0 ? Mathf.Min(rifleDefault, rifleMax) : -1);
                    break;
                case AmmoType.GRENADE:
                    maxAmmoMap[type] = grenadeMax;
                    ammoMap.Add(type, maxAmmoMap[type] >= 0 ? Mathf.Min(grenadeDefault, grenadeMax) : -1);
                    break;
                default:
                    break;
            }
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Excclude ammo from system!
    /// </summary>
    public int GetAmmo(AmmoType type, int required)
    {
        int removed = required;
        if (maxAmmoMap[type] >= 0) {
            removed = Mathf.Min(required, ammoMap[type]);
            ammoMap[type] = ammoMap[type] - removed;
        }
        Debug.Log("removed: " + removed);
        return removed;
    }

    public bool HasAmmo(AmmoType type)
    {
        return maxAmmoMap[type] < 0 || ammoMap[type] > 0;
    }

    public int AddAmmo(AmmoType type, int count)
    {
        int added = 0;
        if (maxAmmoMap[type] >= 0)
        {
            added = Mathf.Min(count, maxAmmoMap[type] - ammoMap[type]);
            ammoMap[type] = ammoMap[type] + added;
        }
        return added;
    }

    public bool IsAmmoFull(AmmoType type)
    {
        return ammoMap[type] == maxAmmoMap[type];
    }

    public AmmoType GetCurrentAmmoType()
    {
        return currentWeapon == null ? AmmoType.NONE : currentWeapon.GetAmmoType();
    }

    public int GetCurrentAmmoInClip()
    {
        return currentWeapon == null ? 0 : currentWeapon.GetAmmoLeft();
    }

    public int GetAmmoLeft(AmmoType type)
    {
        return ammoMap[type];
    }
}
