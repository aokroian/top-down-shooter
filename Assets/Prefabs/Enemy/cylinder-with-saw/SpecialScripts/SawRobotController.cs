using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawRobotController : MonoBehaviour
{
    public GameObject handsRef;
    public GameObject leftSawRef;
    public GameObject rightSawRef;

    public float handsRotSpeed = 1f;
    public float lSawRotSpeed = 1f;
    public float rSawRotSpeed = 1f;

    void FixedUpdate()
    {
        // hands rotation
        if (handsRotSpeed != 0f)
        {
            Rigidbody rb = handsRef.GetComponent<Rigidbody>();
            rb.AddRelativeTorque(0f, handsRotSpeed * Time.deltaTime, 0f);
        }
        // left and right saw rotation
        if (lSawRotSpeed != 0f)
        {
            Rigidbody rb = leftSawRef.GetComponent<Rigidbody>();
            rb.AddRelativeTorque(0f, lSawRotSpeed * Time.deltaTime, 0f);
        }
        if (rSawRotSpeed != 0f)
        {
            Rigidbody rb = rightSawRef.GetComponent<Rigidbody>();
            rb.AddRelativeTorque(0f, rSawRotSpeed * Time.deltaTime, 0f);
        }
    }
}
