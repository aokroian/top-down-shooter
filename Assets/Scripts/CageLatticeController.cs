using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageLatticeController : MonoBehaviour
{
    public float force = 10;
    public GameObject mainCam;

    public void AddForceOppositeOfPlayer()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Transform player = GameObject.Find("Player").transform;

        // disabling joints
        FixedJoint[] joints = GetComponents<FixedJoint>();
        foreach(FixedJoint joint in joints)
        {
            joint.connectedBody = null;
            joint.breakForce = 0;
        }

        if (player == null)
        {
            Debug.Log("Player not found in SimpleForce script in " + gameObject.name + " object");
            return;
        }
        Vector3 direction = gameObject.transform.position - player.position;
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }

        mainCam.GetComponent<SimpleCameraController>().IncrementBrokenCageLatticeCount();
    }
}
