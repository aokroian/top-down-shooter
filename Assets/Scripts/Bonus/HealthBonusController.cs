using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBonusController : AbstractBonusController
{
    public float amount = 20f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override bool OnPickUp()
    {
        return player.GetComponent<Target>().Heal(amount);
    }
}
