using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{
    public GameObject player;

    public float movementRapidity = 0.1f;
    public float minLookAtDistance = 1f;


    private Vector3 offset;
    private PlayerController playerController;

    private Vector3 currentVelocity;
    public enum constraintType
    {
        HandPos,
        HeadPos
    }

    public constraintType constraintSelect;
    // Start is called before the first frame update

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (constraintSelect == constraintType.HandPos)
        {
            
            Vector3 finalAimPoint = playerController.aimAtPosition;
            Vector3 playerPos = new Vector3(player.transform.position.x, finalAimPoint.y, player.transform.position.z);
            if (Vector3.Distance(finalAimPoint, playerPos) < minLookAtDistance)
            {
                Vector3 dir = finalAimPoint - playerPos;
                finalAimPoint = Vector3.Normalize(dir) * minLookAtDistance;
            }

            //transform.position = Vector3.SmoothDamp(transform.position, finalAimPoint, ref currentVelocity, movementRapidity);
            transform.position = finalAimPoint;
        }
    }
}
