using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAim : MonoBehaviour
{
    public bool isOnPlayer;
    public GameObject laserOriginPoint;
    public bool isEnabled;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = laserOriginPoint.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled)
        {
            lineRenderer.enabled = false;
        }
        lineRenderer.SetPosition(0, laserOriginPoint.transform.position);
        //lineRenderer.SetPosition(1, laserOriginPoint.transform.position + laserOriginPoint.transform.forward * 100f);
        RaycastHit hit;
        if (Physics.Raycast(laserOriginPoint.transform.position, laserOriginPoint.transform.forward, out hit, 40))
        {
            if (hit.collider.CompareTag("Player") && !isOnPlayer || hit.collider.CompareTag("Enemy") && isOnPlayer)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                //lineRenderer.SetPosition(1, laserOriginPoint.transform.position + laserOriginPoint.transform.forward * 3f);
                //lineRenderer.SetPosition(1, laserOriginPoint.transform.position);
                lineRenderer.SetPosition(1, laserOriginPoint.transform.position);
            }
        } else
        {
            lineRenderer.SetPosition(1, laserOriginPoint.transform.position);
        }
    }
}
