using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Explosion : MonoBehaviour
{
    public float cameraShakeMagnitude;
    public GameObject explosionEffect;
    public float explosionRadius = 1f;
    public float explosionForce = 1f;
    public float explosionMaxDamage = 200;

    public void Explode()
    {
        Collider[] damagedObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        Vector3 explosivePos = new Vector3(transform.position.x, 1.5f, transform.position.z);

        foreach (var hitCollider in damagedObjects)
        {
            // check if there are obstacles between explosive and target
            RaycastHit hit;
            Vector3 rayDir = hitCollider.transform.position - explosivePos;
            if (Physics.Raycast(explosivePos, rayDir, out hit, explosionRadius))
            {

                Target target = hit.transform.gameObject.GetComponent<Target>();
                if (target != null)
                {
                    // DAMAGE
                    // value to make damage from explosion more natural
                    float amplification = 1.3f;
                    float clampedDist = Mathf.Clamp(Vector3.Distance(transform.position, hitCollider.transform.position), 0f, explosionRadius);
                    float damagePercent = (explosionRadius - clampedDist) / explosionRadius;
                    float clampedDamage = Mathf.Clamp(explosionMaxDamage * damagePercent * amplification, 0f, explosionMaxDamage);
                    target.TakeDamage(clampedDamage, explosionForce, null, true, transform.position, explosionRadius);


                    // FORCE
                    Rigidbody rigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    }
                }
            }


        }
        Instantiate(explosionEffect, transform.position, transform.rotation);
        SimpleCameraController camController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SimpleCameraController>();
        CameraShaker.Instance.ShakeOnce(cameraShakeMagnitude, 4f, 0.1f, 0.5f);
        Destroy(gameObject);
    }
}
