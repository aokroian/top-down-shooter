using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAmmoController : MonoBehaviour, IAmmoProvider
{
    public IAmmoConsumer currentWeapon;

    // -1 for endless
    [Header("Max ammo count (-1 for endless)")]
    public int pistolMax = -1;
    public int machinegunMax;
    public int rifleMax;
    public int shotgunMax;
    public int grenadeMax;

    [Header("Start ammo count")]
    public int pistolDefault = -1;
    public int machinegunDefault;
    public int rifleDefault;
    public int shotgunDefault;
    public int grenadeDefault;

    private Dictionary<AmmoType, int> ammoMap = new Dictionary<AmmoType, int>();
    private Dictionary<AmmoType, int> maxAmmoMap = new Dictionary<AmmoType, int>();

    private Dictionary<AmmoType, float> statPrevFullTime = new Dictionary<AmmoType, float>();
    private Dictionary<AmmoType, int> statAmmoFull = new Dictionary<AmmoType, int>();
    private Dictionary<AmmoType, float> statPrevEmptyTime = new Dictionary<AmmoType, float>();
    private Dictionary<AmmoType, int> statAmmoEmpty = new Dictionary<AmmoType, int>();
    private Dictionary<AmmoType, int> statAmmoSpent = new Dictionary<AmmoType, int>();

    void Start()
    {
        CalcAmmo();

        foreach (AmmoType type in (AmmoType[])System.Enum.GetValues(typeof(AmmoType)))
        {
            statAmmoEmpty[type] = 0;
            statAmmoFull[type] = 0;
            statAmmoSpent[type] = 0;
            statPrevFullTime[type] = Time.time;
            statPrevEmptyTime[type] = Time.time;
        }
    }

    public void CalcAmmo()
    {
        foreach (AmmoType type in (AmmoType[])System.Enum.GetValues(typeof(AmmoType)))
        {
            switch (type)
            {
                case AmmoType.NONE:
                    maxAmmoMap[type] = 0;
                    ammoMap[type] = 0;
                    break;
                case AmmoType.PISTOL:
                    maxAmmoMap[type] = pistolMax;
                    ammoMap[type] = maxAmmoMap[type] >= 0 ? Mathf.Min(pistolDefault, pistolMax) : -1;
                    break;
                case AmmoType.MACHINEGUN:
                    maxAmmoMap[type] = machinegunMax;
                    ammoMap[type] = maxAmmoMap[type] >= 0 ? Mathf.Min(machinegunDefault, machinegunMax) : -1;
                    break;
                case AmmoType.RIFLE:
                    maxAmmoMap[type] = rifleMax;
                    ammoMap[type] = maxAmmoMap[type] >= 0 ? Mathf.Min(rifleDefault, rifleMax) : -1;
                    break;
                case AmmoType.SHOTGUN:
                    maxAmmoMap[type] = shotgunMax;
                    ammoMap[type] = maxAmmoMap[type] >= 0 ? Mathf.Min(shotgunDefault, shotgunMax) : -1;
                    break;
                case AmmoType.GRENADE:
                    maxAmmoMap[type] = grenadeMax;
                    ammoMap[type] = maxAmmoMap[type] >= 0 ? Mathf.Min(grenadeDefault, grenadeMax) : -1;
                    break;
                default:
                    break;
            }
        }
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

        statAmmoSpent[type] += removed;
        if (removed == 0 && required != 0) {
            AddEmptyToStat(type);
        }

        return removed;
    }

    public bool HasAmmo(AmmoType type)
    {
        var result = maxAmmoMap[type] < 0 || ammoMap[type] > 0;
        if (!result) {
            AddEmptyToStat(type);
        }
        return maxAmmoMap[type] < 0 || ammoMap[type] > 0;
    }

    public int AddAmmo(AmmoType type, int count)
    {
        int added = 0;
        if (maxAmmoMap[type] >= 0)
        {
            added = Mathf.Min(count, maxAmmoMap[type] - ammoMap[type]);
            ammoMap[type] = ammoMap[type] + added;
        } else
        {
            AddFullToStat(type);
        }
        return added;
    }

    public bool IsAmmoFull(AmmoType type)
    {
        var result = ammoMap[type] == maxAmmoMap[type];
        if (result) {
            AddFullToStat(type);
        }
        return result;
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

    private void AddFullToStat(AmmoType type)
    {
        if (statPrevFullTime[type] + 5f < Time.time)
        {
            statAmmoFull[type]++;
            statPrevFullTime[type] = Time.time;
        }
    }

    private void AddEmptyToStat(AmmoType type)
    {
        if (statPrevEmptyTime[type] + 5f < Time.time)
        {
            statAmmoEmpty[type]++;
            statPrevEmptyTime[type] = Time.time;
        }
    }

    public Dictionary<string, int> GetAnalyticsValues()
    {
        var result = new Dictionary<string, int>();

        foreach(var pair in statAmmoFull)
        {
            if (pair.Value == 0)
            {
                continue;
            }
            result["full_" + Enum.GetName(typeof(AmmoType), pair.Key)] = pair.Value;
        }
        foreach (var pair in statAmmoEmpty)
        {
            if (pair.Value == 0)
            {
                continue;
            }
            result["empty_" + Enum.GetName(typeof(AmmoType), pair.Key)] = pair.Value;
        }
        foreach (var pair in statAmmoSpent)
        {
            if (pair.Value == 0)
            {
                continue;
            }
            result["spent_" + Enum.GetName(typeof(AmmoType), pair.Key)] = pair.Value;
        }

        return result;
    }
}
