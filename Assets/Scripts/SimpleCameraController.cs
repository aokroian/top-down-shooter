using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public GameObject playerRef;
    
    public float playerToPointerPart = 0.2f;
    public float smoothTime = 0.2f;

    // offset vars
    public Vector3 normalOffset;
    public Vector3 cageOffset;
    private Vector3 offset;

    // for start pos player cage
    public int maxBrokenCageLatticeCountBeforeCameraUp = 3;
    private int brokenCageLatticeCount = 0;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        offset = cageOffset;
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
        var target = Vector3.Lerp(playerRef.transform.position, new Vector3(playerAimingPoint.x, 0f, playerAimingPoint.z), playerToPointerPart) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }


    public void IncrementBrokenCageLatticeCount()
    {
        brokenCageLatticeCount++;
        if (brokenCageLatticeCount > maxBrokenCageLatticeCountBeforeCameraUp)
        {
            offset = normalOffset;
        }
    }

}
