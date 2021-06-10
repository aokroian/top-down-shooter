using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject hitEffectRef;
    public float bulletImpactForce;
    public float shotDamage;

    public float velocity = 10.0f;

    private Vector3 direction;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) >= 20f)
        {
            Destroy(gameObject);
        } else
        {
            transform.Translate(direction * Time.deltaTime, Space.World);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction * velocity;
        //Debug.Log(direction);
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("collision: " + collision.gameObject.name);
        // частицы на цели (потом нужно убрать логику в саму цель, чтобы были разные)
        //Instantiate(hitEffectRef, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

        // урон объекту
        if (collision.gameObject.name != "Player" && collision.gameObject.name != "Weapon" && collision.gameObject.name != "BulletOutPoint")
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
