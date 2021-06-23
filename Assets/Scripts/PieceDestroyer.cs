using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceDestroyer : MonoBehaviour
{
    public float timeToLive = Mathf.Infinity;
    public float massAfterCollision = 0.001f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;


        if (timeToLive <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().mass = massAfterCollision;
    }
}
