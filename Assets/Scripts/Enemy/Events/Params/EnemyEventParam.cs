using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct?
public class EnemyEventParam
{
    public Vector3 position;
    public float cost;

    public EnemyEventParam(Vector3 position, float cost)
    {
        this.position = position;
        this.cost = cost;
    }
}
