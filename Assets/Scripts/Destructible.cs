using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destructiblePrefab;
    private Target targetComponent;
    // Start is called before the first frame update
    void Start()
    {
        targetComponent = GetComponent<Target>();
    }

    // Update is called once per frame
    public void ChangeToDestructible()
    {
        Instantiate(destructiblePrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }


}
