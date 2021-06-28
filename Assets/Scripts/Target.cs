using UnityEngine;
using UnityEngine.Events;
using EZCameraShake;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public GameObject onHitEffect;

    public UnityEvent onDeath;

    public bool isDead { get; private set; }

    // variables to store data for destructible objects (like robots)
    [HideInInspector]
    public Vector3 hitDir;
    [HideInInspector]
    public float hitForceAmount;
    [HideInInspector]
    public bool isFromExplosion;
    [HideInInspector]
    public Vector3 explosionPosition;
    [HideInInspector]
    public float explosionRadius;

    public void TakeDamage(float amount, float? forceAmount = null, Vector3? hitDirection = null, bool? isHitFromExplosion = null, Vector3? explPosition = null, float? explRadius = null)
    {
        health -= amount;

        if (gameObject.tag == "Player")
        {
            CameraShaker.Instance.ShakeOnce(2f, 5f, 0.05f, 0.05f);
        }

        // store last hit parameters 
        hitDir = hitDirection ?? Vector3.zero;
        hitForceAmount = forceAmount ?? 0f;
        isFromExplosion = isHitFromExplosion ?? false;
        explosionPosition = explPosition ?? Vector3.zero;
        explosionRadius = explRadius ?? 0f;
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
