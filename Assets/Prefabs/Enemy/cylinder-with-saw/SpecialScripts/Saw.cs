using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public GameObject robotRef;

    private void OnCollisionEnter(Collision collision)
    {
        robotRef.GetComponent<EnemyController>().Bite();
    }
}
