using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public GameObject robotRef;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            robotRef.GetComponent<EnemyController>().Bite();
        }
    }
}
