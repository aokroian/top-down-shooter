using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destructiblePrefab;

    private Dictionary<string, Vector3> partsPos = new Dictionary<string, Vector3>()
    {

    };
    private Dictionary<string, Quaternion> partsRot = new Dictionary<string, Quaternion>()
    {
    };

    private void Start()
    {

    }
    public void ChangeToDestructible()
    {
        Target target = gameObject.GetComponent<Target>();

        GameObject destructible = Instantiate(destructiblePrefab, transform.position, transform.rotation);


        //force to all parts
        foreach (Transform child in destructible.transform)
        {
            Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
            Vector3 force = Vector3.ClampMagnitude(target.hitDir, 1f) * target.hitForceAmount;

            if (target.isFromExplosion)
            {
                rb.AddExplosionForce(target.hitForceAmount, target.explosionPosition, target.explosionRadius);
            } else
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
            
        }
        Destroy(gameObject);
    }
}
