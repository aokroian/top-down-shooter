using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ThrowableItemController : MonoBehaviour
{
    // variables needed for instantiation
    public GameObject ownerObjRef;
    public Vector3 localPosition;
    public Vector3 localRotation;
    public Vector3 localScale;

    public float timeToLiveAfterThrow = 1f;

    // variables for explosion only
    public float cameraShakeMagnitude;
    public bool shouldExplode = false;
    public GameObject explosionEffect;
    public float explosionRadius = 1f;
    public float explosionForce = 1f;
    public float explosionMaxDamage = 200;
    public float explosionDurationTimer = 0f;
    public bool isThrowed = false;


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
                    Explode();
                }
                else
                {
                    Destroy(gameObject);
                }

            }
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
        Collider[] damagedObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        Vector3 grenadePos = new Vector3(transform.position.x, 1.5f, transform.position.z);

        foreach (var hitCollider in damagedObjects)
        {
            // check if there are obstacles between grenade and target
            RaycastHit hit;
            Vector3 rayDir = hitCollider.transform.position - grenadePos;
            if (Physics.Raycast(grenadePos, rayDir, out hit, explosionRadius))
            {
                // DAMAGE
                Target target = hit.transform.gameObject.GetComponent<Target>();
                if (target != null)
                {
                    // value to make damage from explosion more natural
                    float amplification = 1.3f;
                    float clampedDist = Mathf.Clamp(Vector3.Distance(transform.position, hitCollider.transform.position), 0f, explosionRadius);
                    float damagePercent = (explosionRadius - clampedDist) / explosionRadius;
                    float clampedDamage = Mathf.Clamp(explosionMaxDamage * damagePercent * amplification, 0f, explosionMaxDamage);
                    target.TakeDamage(clampedDamage);
                }

                // FORCE
                Rigidbody rigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
        }
        Instantiate(explosionEffect, transform.position, transform.rotation);
        SimpleCameraController camController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SimpleCameraController>();
        CameraShaker.Instance.ShakeOnce(cameraShakeMagnitude, 4f, 0.1f, 0.5f);
        Destroy(gameObject);
    }

    
}
