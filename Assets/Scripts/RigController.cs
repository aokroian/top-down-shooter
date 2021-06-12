using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{
    public GameObject player;
    public Vector3 controllerDefaultPos;
    public Vector3 controllerDefaultRot;

    public float movementRapidity = 0.1f;

    private Vector3 offset;
    private PlayerController playerController;

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
            offset = new Vector3(0.0f, 1.5f, 0.0f);
            if (playerController.isAiming)
            {
                transform.position = Vector3.Lerp(transform.position, playerController.aimPosition + offset, movementRapidity);
                //transform.LookAt(playerController.mousePosOnGround + offset);
            } else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, controllerDefaultPos, movementRapidity);
                //transform.position = player.transform.TransformPoint(controllerDefaultPos);
                //transform.rotation = Quaternion.Euler(controllerDefaultRot);
            }
            
        }

        if (constraintSelect == constraintType.HeadPos)
        {
            offset = new Vector3(0.0f, 1f, 0.0f);
            transform.LookAt(playerController.aimPosition + offset);
            
        }

    }
}
