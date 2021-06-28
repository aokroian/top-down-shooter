using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour, IBulletController
{
    [InspectorName("Bullet params")]
    public BulletParams bp;


    // Start is called before the first frame update
    void Start()
    {
        bp.player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(bp.player.transform.position, transform.position) >= bp.despawnDistance)
        {
            Destroy(gameObject);
        } else
        {
            transform.Translate(bp.velocity * Time.deltaTime, Space.World);
        }
    }

    private void OnEnable()
    {
        AwareEventParam param = new AwareEventParam(transform.position, bp.awareDistance);
        bp.awareEvent.Raise(param);
    }

    public void SetVelocity(Vector3 velocity)
    {
        bp.velocity = velocity;
        //Debug.Log(direction);
    }

    public void SetShooter(GameObject shooter)
    {
        bp.shooter = shooter;
    }

    public void SetDamage(float damage)
    {
        bp.shotDamage = damage;
    }

    public void SetImpactForce(float force)
    {
        bp.bulletImpactForce = force;
    }

    public void SetHitEffectRef(GameObject hitEffectRef)
    {
        bp.hitEffectRef = hitEffectRef;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTrigger: " + other.gameObject.name);
        
        if (bp.shooter == other.gameObject || other.CompareTag(tag))
        {
            return;
        }
        

        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy" || other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            
            Target target = other.transform.GetComponent<Target>();
            if (target != null)
            {
                SpawnEffect(target);
                target.TakeDamage(bp.shotDamage, bp.bulletImpactForce, -(bp.shooter.transform.position - transform.position));
            }

            /*
            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.AddForceAtPosition(-collision.contacts[0].normal * bulletImpactForce, collision.contacts[0].point);
            }
            */

            AwareEventParam param = new AwareEventParam(transform.position, bp.awareDistance);
            bp.awareEvent.Raise(param);

            Destroy(gameObject);
        }
    }

    private void SpawnEffect(Target target)
    {
        Vector3 dir = transform.position - target.transform.position;
        dir.y = 0f;
        var effect = target.onHitEffect == null ? bp.hitEffectRef : target.onHitEffect;
        Instantiate(effect, transform.position, Quaternion.LookRotation(dir));
    }
}
