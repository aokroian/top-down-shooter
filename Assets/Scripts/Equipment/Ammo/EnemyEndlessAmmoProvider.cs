using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEndlessAmmoProvider : IAmmoProvider
{
    public int GetAmmo(AmmoType type, int required)
    {
        return required;
    }

    public bool HasAmmo(AmmoType type)
    {
        return true;
    }
}
