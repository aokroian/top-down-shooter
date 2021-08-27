using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceDestroyer : MonoBehaviour
{
    public float timeToLive = Mathf.Infinity;
    public float massAfterCollisionWithPlayer = 0.001f;

    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>()?.material;
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;

        if (material == null) return;
        // dissolve shader update 
        if (material.HasFloat("Dissolve"))
        {
            float current = material.GetFloat("Dissolve");
            material.SetFloat("Dissolve", current += Time.deltaTime/timeToLive);
        }

        if (timeToLive <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            GetComponent<Rigidbody>().mass = massAfterCollisionWithPlayer;
    }
}
