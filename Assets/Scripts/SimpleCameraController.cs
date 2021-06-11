using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public GameObject playerRef;
    public Vector3 offset;
    public float smoothTime = 0.01f;

    public float zAxisInstantOffset = -4f;
    public float yAxisInstantOffset = 10f;
    public float zAxisPositiveOffsetLimit = 4f;
    public float zAxisNegativeOffsetLimit = -1f;
    public float xAxisOffsetLimit = 4f;

    private Vector3 playerAimingPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        //transform.position = playerRef.transform.position + offset;
        playerAimingPoint = playerRef.GetComponent<PlayerController>().mousePosOnGround;


        //transform.Translate(playerAimingPoint * smoothTime);


        if (transform.position.z >= playerRef.transform.position.z + zAxisPositiveOffsetLimit + zAxisInstantOffset)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zAxisPositiveOffsetLimit + playerRef.transform.position.z + zAxisInstantOffset);
        }
        if (transform.position.z <= playerRef.transform.position.z + zAxisNegativeOffsetLimit + zAxisInstantOffset)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerRef.transform.position.z + zAxisNegativeOffsetLimit + zAxisInstantOffset);
        }


        if (transform.position.x >= playerRef.transform.position.x + xAxisOffsetLimit)
        {
            transform.position = new Vector3(playerRef.transform.position.x + xAxisOffsetLimit, transform.position.y, transform.position.z );
        }
        if (transform.position.x <= playerRef.transform.position.x - xAxisOffsetLimit)
        {
            transform.position = new Vector3(playerRef.transform.position.x - xAxisOffsetLimit, transform.position.y, transform.position.z);
        }


        
        //transform.position = Vector3.LerpUnclamped(transform.position, new Vector3(playerAimingPoint.x, yAxisInstantOffset, playerAimingPoint.z), smoothTime);
    }
}
