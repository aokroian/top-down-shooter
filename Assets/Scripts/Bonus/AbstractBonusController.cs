using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBonusController : MonoBehaviour
{
    public GameObject player;
    public float despawnDistance = 80f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) >= despawnDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if (OnPickUp())
            {
                DestroyPickedUp();
            }
        }
    }

    public abstract bool OnPickUp();

    protected void DestroyPickedUp()
    {
        Destroy(gameObject);
    }
}
