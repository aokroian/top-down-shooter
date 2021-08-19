using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public GameObject playerRef;
    
    public float playerToPointerPart = 0.2f;
    public float cagePlayerToPointerPart = 0.05f;
    public float smoothTime = 0.2f;

    // offset vars
    public Vector3 normalOffset;
    public Vector3 cageOffset;
    private Vector3 offset;

    // for player cage
    public float centerToPlayerRadiusToRaiseCam = 2;
    public bool hasAlreadyBeenRaised = false;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        offset = cageOffset;
    }

    private void LateUpdate()
    {
        float lerpPart = playerToPointerPart;
        if (hasAlreadyBeenRaised == false && playerRef.transform.position.magnitude < centerToPlayerRadiusToRaiseCam)
        {
            offset = cageOffset;
            lerpPart = cagePlayerToPointerPart;
        } else
        {
            offset = normalOffset;
            hasAlreadyBeenRaised = true;
        }
        var playerAimingPoint = playerRef.GetComponent<PlayerController>().aimAtPosition;
        var target = Vector3.Lerp(playerRef.transform.position, new Vector3(playerAimingPoint.x, 0f, playerAimingPoint.z), lerpPart) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }

}
