using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleController : MonoBehaviour
{
    public GameObject hitEffectRef;
    public float bulletImpactForce;
    public float shotDamage;

    private void OnCollisionEnter(Collision collision)
    {

        // частицы на цели (потом нужно убрать логику в саму цель, чтобы были разные)
        Instantiate(hitEffectRef, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

        // урон объекту
        if (collision.gameObject.name != "BulletOutPoint" && collision.gameObject.name != "Player")
        {
            Target target = collision.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(shotDamage);
            }
            // толчок пулей на объект попадания
            if (collision.rigidbody != null)
            {
                collision.rigidbody.AddForceAtPosition(-collision.contacts[0].normal * bulletImpactForce, collision.contacts[0].point);
            }

            Destroy(gameObject, 0.02f);
        }

    }
}
