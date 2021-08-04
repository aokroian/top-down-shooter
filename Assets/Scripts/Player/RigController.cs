using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{
    public GameObject player;
    public float minLookAtDistance = 1f;
    public float autoAimAngle = 15f;
    public float maxAutoAimDistance = 25f;
    public GameObject enemiesContainer;

    private PlayerController playerController;

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
        var enemiesTransform = enemiesContainer.transform;
        for (int i = 0; i < enemiesTransform.childCount; i++)
        {
            var child = enemiesTransform.GetChild(i);
            Vector3 toEnemy = child.transform.position - player.transform.position;
            float angle = Mathf.Abs(Vector3.Angle(aimVector, toEnemy));
            RaycastHit hitInfo;
            bool isHit = Physics.Linecast(player.transform.position, child.position, out hitInfo, LayerMask.GetMask("Obstacle"));
            if (angle < autoAimAngle
                && angle < currentAngle
                && Vector3.Distance(player.transform.position, child.position) < maxAutoAimDistance
                && child.gameObject.GetComponentInChildren<Renderer>().isVisible
                && !isHit)
            {
                currentAimEnemy = child;
                currentAngle = angle;
            }
        }

        return currentAimEnemy;
    }
}
