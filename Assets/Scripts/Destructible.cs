using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Destructible : MonoBehaviour
{
    public GameObject destructiblePrefab;

    private Vector3? velocity;
    private Target target;
    private void Start()
    {
        target = gameObject.GetComponent<Target>();
    }
    public void ChangeToDestructible()
    {
        GameObject destructible = Instantiate(destructiblePrefab, transform.position, transform.rotation);
        velocity = gameObject.GetComponent<NavMeshAgent>().velocity;

        //force to all parts
        foreach (Transform child in destructible.transform)
        {
            Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
            Vector3 force = (Vector3.ClampMagnitude(target.hitDir, 1f) * target.hitForceAmount) ;

            if (target.isFromExplosion)
            {
                rb.AddExplosionForce(target.hitForceAmount, target.explosionPosition, target.explosionRadius);
            } else
            {
                rb.AddForce(force + (velocity ?? Vector3.zero), ForceMode.Impulse);
            }
            
        }
        Destroy(gameObject);
    }
}
