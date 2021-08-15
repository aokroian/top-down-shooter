using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwareEventParam
{
    public Vector3 position;
    public float distance;

    public AwareEventParam(Vector3 position, float distance)
    {
        this.position = position;
        this.distance = distance;
    }
}
