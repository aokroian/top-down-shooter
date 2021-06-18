using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;

    public UnityEvent onDeath;

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
            onDeath.Invoke();
            Destroy(gameObject, 0.02f);
        }
    }

    public float GetHPPercent()
    {
        return health / maxHealth;
    }
}
