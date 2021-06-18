using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItemController : MonoBehaviour
{
    // variables needed for instantiation
    public GameObject ownerObjRef;
    public Vector3 localPosition;
    public Vector3 localRotation;
    public Vector3 localScale;

    public float timeToLiveAfterThrow = 1f;

    // variables for explosion only
    public bool shouldExplode = false;

    public float explosionRadius = 1f;
    public float explosionForce = 1f;
    public float explosionMaxDamage = 200;
    public float explosionDuration = 0.01f;
    public float explosionDurationTimer = 0f;


    private bool isThrowed = false;


    private void Update()
    {


        // after item is throwed
        if (isThrowed)
        {
            timeToLiveAfterThrow -= Time.deltaTime;

            if (timeToLiveAfterThrow <= 0f)
            {
                if (shouldExplode)
                {
                    explosionDurationTimer = explosionDuration;
                    Explode();
                } else
                {
                    Destroy(gameObject);
                }
                
            }
        }

        // only for items that should explode
        // after item is throwed and has no more time to live
        if (explosionDurationTimer > 0f)
        {
            explosionDurationTimer -= Time.deltaTime;
        }
        if (explosionDurationTimer <= 0f && timeToLiveAfterThrow <=0)
        {
            Destroy(gameObject);
        }

    }


    public void Throw(Vector3 appliedForce)
    {
        gameObject.transform.SetParent(GameObject.Find("Map").transform);

        if (gameObject.AddComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }

        gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.GetComponent<Rigidbody>().AddForce(appliedForce, ForceMode.Impulse);

        isThrowed = true;
    }

    private void Explode()
    {
        
    }
}
