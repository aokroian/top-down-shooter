using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public GameObject playerRef;
    public Vector3 offset;
    public float playerToPointerPart = 0.2f;
    public float smoothTime = 0.2f;
    

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        /*
        var playerAimingPoint = playerRef.GetComponent<PlayerController>().mousePosOnGround;
        var target = Vector3.Lerp(playerRef.transform.position, new Vector3(playerAimingPoint.x, 0.0f, playerAimingPoint.z), playerToPointerPart) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
        */
    }
    
    private void LateUpdate()
    {
        var playerAimingPoint = playerRef.GetComponent<PlayerController>().aimAtPosition;
        var target = Vector3.Lerp(playerRef.transform.position, new Vector3(playerAimingPoint.x, 0.0f, playerAimingPoint.z), playerToPointerPart) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }
    
    
}
