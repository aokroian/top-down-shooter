using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLavaController : MonoBehaviour
{
    public float expandSpeed = 1;
    public Material material;

    // Update is called once per frame
    void Update()
    {
        /*
        var renderers = transform.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer v in renderers)
        {
            v.material.SetFloat("OuterRadius", Time.time * expandSpeed);
        }
        */
        material.SetFloat("OuterRadius", Time.time * expandSpeed);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(Vector3.zero, Time.time * expandSpeed);

    }
}
