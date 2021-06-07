using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemySpawner))]
public class EnemySpawnerManager: MonoBehaviour
{
    public GameObject player;

    private EnemySpawner spawner;
    
    void Start()
    {
        spawner = GetComponent<EnemySpawner>();
        StartCoroutine("TEMPStartCounter");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(GameObject groundTile)
    {
        spawner.Spawn(groundTile, 5, 1, 2);
    }


    IEnumerator TEMPStartCounter()
    {
        yield return new WaitForSeconds(3f);
        Spawn(GameObject.Find("Ground"));
        StartCoroutine("TEMPStartCounter");
    }
}
