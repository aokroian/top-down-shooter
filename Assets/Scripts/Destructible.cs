using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Destructible : MonoBehaviour
{
    public GameObject destructiblePrefab;
    public Explosion explosion;

    private Vector3? velocity;
    private Target target;
    private void Start()
    {
        target = gameObject.GetComponent<Target>();
        if (explosion == null)
        {
            explosion = GetComponent<Explosion>();
        }
    }
    public void ChangeToDestructible()
    {
        GameObject destructible = Instantiate(destructiblePrefab, transform.position, transform.rotation);
        velocity = gameObject.GetComponent<NavMeshAgent>().velocity;

        //force to all parts
        foreach (Transform child in destructible.transform)
        {
            // change parent node
            //child.transform.SetParent(transform.parent);

            Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
            Vector3 force = (Vector3.ClampMagnitude(target.hitDir, 1f) * target.hitForceAmount) ;

            if (target.isFromExplosion)
            {
                rb.AddExplosionForce(target.hitForceAmount, target.explosionPosition, target.explosionRadius);
            }
            else
            {
                rb.AddForce(force + (velocity ?? Vector3.zero), ForceMode.Impulse);
            }

        }
        if (explosion != null)
        {
            explosion.Explode();
        } else
        {
            Destroy(gameObject);
        }
        
    }
}
