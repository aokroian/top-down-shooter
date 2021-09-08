using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public float damagePerSecond = 1;
    public float areaRadius = 3;
    public bool givesInvulnerability = false;

    [HideInInspector]
    public bool isEnabled = false;

    private Vector3 pos;
    private Target ownerTarget;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<Target>(out ownerTarget);
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        if (isEnabled)
        {
            // damage
            Collider[] damagedObjects = Physics.OverlapSphere(pos, areaRadius);
            foreach (var hitCollider in damagedObjects)
            {
                Target target;
                hitCollider.gameObject.TryGetComponent<Target>(out target);
                if (target != null && target.CompareTag("Enemy"))
                {
                    target.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
            // invulnerability
            if (givesInvulnerability && ownerTarget)
            {
                ownerTarget.invulnerability = true;
            }
        }
        else if(!isEnabled)
        {
            if (ownerTarget)
            {
                ownerTarget.invulnerability = false;
            }
        }
    }
}
