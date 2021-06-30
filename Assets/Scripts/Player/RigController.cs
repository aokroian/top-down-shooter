using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{
    public GameObject player;
    public float minLookAtDistance = 1f;

    private PlayerController playerController;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 finalAimPoint = playerController.aimAtPosition;
        Vector3 playerPos = new Vector3(player.transform.position.x, finalAimPoint.y, player.transform.position.z);
        if (Vector3.Distance(finalAimPoint, playerPos) < minLookAtDistance)
        {
            Vector3 dir = finalAimPoint - playerPos;
            finalAimPoint = Vector3.Normalize(dir) * minLookAtDistance;
        }
        transform.position = new Vector3(finalAimPoint.x, 1.57f, finalAimPoint.z);
    }
}
