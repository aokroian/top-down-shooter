using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{
    public GameObject player;
    public float minLookAtDistance = 1f;
    public float autoAimAngle = 15f;
    public float maxAutoAimDistance = 25f;
    public float distancePriorityWeightMultiplier = 1.0f;
    public float prevTargetWeightBonus = 10.0f;
    public GameObject enemiesContainer;

    private PlayerController playerController;
    private Transform prevTarget;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 finalAimPoint = AutoAim(playerController.aimAtPosition);
        Vector3 playerPos = new Vector3(player.transform.position.x, finalAimPoint.y, player.transform.position.z);
        if (Vector3.Distance(finalAimPoint, playerPos) < minLookAtDistance)
        {
            Vector3 dir = finalAimPoint - playerPos;
            finalAimPoint = playerPos + Vector3.Normalize(dir) * minLookAtDistance;
        }
        transform.position = new Vector3(finalAimPoint.x, 1.57f, finalAimPoint.z);
    }

    private Vector3 AutoAim(Vector3 aimPoint)
    {
        Vector3 aimFloorPosition = new Vector3(aimPoint.x, 0f, aimPoint.z);
        var enemy = GetEnemyAimTo(aimFloorPosition - player.transform.position);
        return enemy == null ? aimPoint : enemy.position;
    }

    private Transform GetEnemyAimTo(Vector3 aimVector)
    {
        Transform currentAimEnemy = null;
        float currentAngle = 180f;
        float currentWeight = float.MaxValue;
        var enemiesTransform = enemiesContainer.transform;
        for (int i = 0; i < enemiesTransform.childCount; i++)
        {
            var child = enemiesTransform.GetChild(i);
            Vector3 toEnemy = child.transform.position - player.transform.position;
            float angle = Mathf.Abs(Vector3.Angle(aimVector, toEnemy));
            if (angle < autoAimAngle
                && angle < currentAngle
                && Vector3.Distance(player.transform.position, child.position) < maxAutoAimDistance
                && child.gameObject.GetComponentInChildren<Renderer>().isVisible
                && !Physics.Linecast(player.transform.position, child.position, LayerMask.GetMask("Obstacle")))
            {
                float weight = CalcWeight(angle, child);
                if (weight < currentWeight) {
                    currentAimEnemy = child;
                    currentAngle = angle;
                    currentWeight = weight;
                    prevTarget = child;
                }
            }
        }

        if (currentAimEnemy == null)
        {
            prevTarget = null;
        }

        return currentAimEnemy;
    }

    // Lower - better
    // Can be negative
    private float CalcWeight(float angle, Transform enemy)
    {
        float result = angle;

        if (enemy == prevTarget)
        {
            result -= prevTargetWeightBonus;
        }

        var distance = Vector3.Distance(player.transform.position, enemy.position);
        result += distance * distancePriorityWeightMultiplier;

        return result;
    }
}
