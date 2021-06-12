using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject hitEffectRef;
    public float bulletImpactForce;
    public float shotDamage;

    public float despawnDistance = 40f;

    private Vector3 velocity;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) >= despawnDistance)
        {
            Destroy(gameObject);
        } else
        {
            transform.Translate(velocity * Time.deltaTime, Space.World);
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
        //Debug.Log(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTrigger: " + other.gameObject.name);
        // частицы на цели (потом нужно убрать логику в саму цель, чтобы были разные)
        Instantiate(hitEffectRef, transform.position, Quaternion.LookRotation(transform.position - other.transform.position));

        // урон объекту
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy" || other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            
            Target target = other.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(shotDamage);
            }

            // толчок пулей на объект попадания
            /*
            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.AddForceAtPosition(-collision.contacts[0].normal * bulletImpactForce, collision.contacts[0].point);
            }
            */

            Destroy(gameObject);
        }
    }
}
