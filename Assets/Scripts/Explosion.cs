using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Explosion : MonoBehaviour
{
    //timer is needed if you need to explode without an event
    public float timeBeforeForcedExplosion = Mathf.Infinity;
    public bool destroyAfterExplosion = true;
    public float cameraShakeMagnitude;
    public GameObject explosionEffect;
    public float explosionRadius = 1f;
    public float explosionForce = 1f;
    public float explosionMaxDamage = 200;
    public float minExplosionHeight = 1f;
    public float maxExplosionHeight = 1f;

    private void Start()
    {
        if (timeBeforeForcedExplosion < Mathf.Infinity)
        {
            StartCoroutine(Countdown());
        }
    }

    public void Explode()
    {
        Collider[] damagedObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        Vector3 explosivePos = new Vector3(Random.Range(transform.position.x - 0.2f, transform.position.x + 0.2f), Random.Range(minExplosionHeight, maxExplosionHeight), Random.Range(transform.position.z - 0.2f, transform.position.z + 0.2f));

        foreach (var hitCollider in damagedObjects)
        {
            Target hitTarget;
            hitCollider.gameObject.TryGetComponent<Target>(out hitTarget);
            float clampedDist = Mathf.Clamp(Vector3.Distance(transform.position, hitCollider.transform.position), 0f, explosionRadius);
            float damagePercent = (explosionRadius - clampedDist) / explosionRadius;
            float clampedDamage = Mathf.Clamp(explosionMaxDamage * damagePercent, 0f, explosionMaxDamage);
            if (hitTarget != null)
            {
                hitTarget.TakeDamage(clampedDamage, explosionForce, null, true, transform.position, explosionRadius);
            }
           

            // check if there are obstacles between explosive and target
            //RaycastHit hit;
            //Vector3 rayDir = hitCollider.transform.position - explosivePos;
            //if (Physics.Raycast(explosivePos, rayDir, out hit, explosionRadius, ~LayerMask.GetMask("Obstacle")))
            //{

            //    Target target = hit.transform.gameObject.GetComponent<Target>();
            //    if (target != null)
            //    {
            //        // DAMAGE
            //        // value to make damage from explosion more natural
            //        float clampedDist = Mathf.Clamp(Vector3.Distance(transform.position, hitCollider.transform.position), 0f, explosionRadius);
            //        float damagePercent = (explosionRadius - clampedDist) / explosionRadius;
            //        float clampedDamage = Mathf.Clamp(explosionMaxDamage * damagePercent, 0f, explosionMaxDamage);
            //        target.TakeDamage(clampedDamage, explosionForce, null, true, transform.position, explosionRadius);
            //    }

            //    // FORCE
            //    Rigidbody rigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
            //    if (rigidbody != null)
            //    {
            //        rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            //    }
            //}


        }
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }
        CameraShaker.Instance.ShakeOnce(cameraShakeMagnitude, 4f, 0.1f, 0.5f);
        if (destroyAfterExplosion)
        {
            Destroy(gameObject);
        }
        
    }

    private IEnumerator Countdown()
    {
        while (timeBeforeForcedExplosion >= 0)
        {
            yield return new WaitForSeconds(1);
            timeBeforeForcedExplosion--;
        }
        Explode();
    }
}
