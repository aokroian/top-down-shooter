using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletParams
{
    public float despawnDistance = 40f;
    public EnemyAwareEvent awareEvent;
    public float awareDistance = 10f;

    [HideInInspector]
    public float shotDamage;
    [HideInInspector]
    public float bulletImpactForce;
    [HideInInspector]
    public GameObject hitEffectRef;
    [HideInInspector]
    public GameObject shooter;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public GameObject player;
}
