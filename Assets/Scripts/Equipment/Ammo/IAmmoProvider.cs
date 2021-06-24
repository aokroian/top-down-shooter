using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmoProvider
{
    public int GetAmmo(AmmoType type, int required);

    public bool HasAmmo(AmmoType type);
}
