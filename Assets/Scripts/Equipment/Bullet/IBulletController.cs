using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletController
{
    public void SetVelocity(Vector3 velocity);

    public void SetShooter(GameObject shooter);

    public void SetDamage(float damage);

    public void SetImpactForce(float force);

    public void SetHitEffectRef(GameObject hitEffectRef);

    public void SetNubmerOfPenetrations(int numberOfPenetrations);
}
