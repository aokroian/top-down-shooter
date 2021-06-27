using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSprayController : MonoBehaviour, IBulletController
{
    public BulletParams bp;
    public int grainCount = 5;
    public float shotAngle = 30f;
    public GameObject grainPrefab;

    public void SetDamage(float damage)
    {
        bp.shotDamage = damage;
    }

    public void SetHitEffectRef(GameObject hitEffectRef)
    {
        bp.hitEffectRef = hitEffectRef;
    }

    public void SetImpactForce(float force)
    {
        bp.bulletImpactForce = force;
    }

    public void SetShooter(GameObject shooter)
    {
        bp.shooter = shooter;
    }

    public void SetVelocity(Vector3 velocity)
    {
        bp.velocity = velocity;
    }

    private void Start()
    {
        float startAngle = 0 - (shotAngle / 2f);
        float step = shotAngle / (grainCount - 1);
        for (int i = 0; i < grainCount; i ++)
        {
            var angle = CalcGrainAngle(startAngle, step, i);
            var velocity = angle * bp.velocity;
            var bullet = Instantiate(grainPrefab, transform.position, Quaternion.LookRotation(Vector3.up, velocity), transform);
            var bulletController = bullet.GetComponent<IBulletController>();
            bulletController.SetImpactForce(bp.bulletImpactForce);
            bulletController.SetDamage(bp.shotDamage);
            bulletController.SetHitEffectRef(bp.hitEffectRef);
            bulletController.SetShooter(bp.shooter);
            bulletController.SetVelocity(velocity);
        }
    }

    private void Update()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }

    private Quaternion CalcGrainAngle(float startAngle, float step, int index)
    {
        float delta = Random.Range(0 - step / 4f, step / 4f);
        return Quaternion.Euler(0f, startAngle + (step * index) + delta, 0f);
    }
}
