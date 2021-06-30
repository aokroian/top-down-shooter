using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAim : MonoBehaviour
{

    
    public GameObject laserOriginPoint;
    public bool isEnabled = false;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = laserOriginPoint.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lineRenderer.enabled = isEnabled;

        lineRenderer.SetPosition(0, laserOriginPoint.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, laserOriginPoint.transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider)
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, laserOriginPoint.transform.position + laserOriginPoint.transform.forward * 100f);
            }
        }
    }
}
