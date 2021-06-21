using UnityEngine;
using UnityEngine.Events;
using EZCameraShake;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;

    public UnityEvent onDeath;

    public bool isDead {get; private set;}

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (gameObject.tag == "Player")
        {
            CameraShaker.Instance.ShakeOnce(2f, 5f, 0.05f, 0.05f);
        }
    }

    public bool Heal(float amount)
    {
        bool result = false;
        if (health < maxHealth)
        {
            health = Mathf.Min(health + amount, maxHealth);
            result = true;
        }
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0f)
        {
            isDead = true;
            onDeath.Invoke();
            Destroy(gameObject, 0.02f);
        }
    }

    public float GetHPPercent()
    {
        return health / maxHealth;
    }
}
