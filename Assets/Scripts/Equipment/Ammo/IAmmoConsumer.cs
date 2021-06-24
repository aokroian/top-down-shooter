using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmoConsumer
{
    public int GetAmmoLeft();

    public AmmoType GetAmmoType();
}
