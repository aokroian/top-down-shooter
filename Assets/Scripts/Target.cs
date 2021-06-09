using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
        
    public void TakeDamage(float amount)
    {
        health -= amount;
    }
    // Update is called once per frame
    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0f)
        {
            Destroy(gameObject, 0.02f);
        }
    }
}
