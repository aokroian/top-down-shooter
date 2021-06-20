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
        isEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.enabled = isEnabled;

        lineRenderer.SetPosition(0, laserOriginPoint.transform.position);

        RaycastHit hit;
        //Vector3 dir = aimAtPoint.transform.position - transform.position;
        if (Physics.Raycast(transform.position, laserOriginPoint.transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider)
            {
                lineRenderer.SetPosition(1, hit.point);
            }
        }
    }
}
