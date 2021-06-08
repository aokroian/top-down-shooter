using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float minY = 0.7f;
    public float noiseMultiplier = 1.11f;

    [Range(0, 999999)]
    public int noiseSeed;
    public bool randomSeed = false;

    private float obstacleSize;
    private float yPos;

    private float noiseOffset = 10000;

    void Start()
    {
        if (randomSeed)
        {
            noiseSeed = Mathf.RoundToInt(Random.Range(0, 999999));
        }
        obstacleSize = obstaclePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * obstaclePrefab.transform.localScale.z;
        yPos = (obstaclePrefab.transform.position.y - obstaclePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.min.y) * obstaclePrefab.transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnObstacles(GameObject groundTile)
    {
        

        var bounds = groundTile.GetComponent<MeshRenderer>().bounds;
        Debug.Log("Size: " + bounds.size.z);

        var maxCount = Mathf.Floor(bounds.size.z / obstacleSize);
        var iterStep = bounds.size.z / maxCount;   

        for (int x = 0; x < maxCount; x ++)
        {
            var xPos = bounds.min.x + iterStep * x;
            for (int z = 0; z < maxCount; z ++)
            {
                var zPos = bounds.min.z + iterStep * z;
                var y = Mathf.PerlinNoise(noiseOffset + xPos * noiseMultiplier, noiseOffset + zPos * noiseMultiplier);
                if (y > minY)
                {
                    Vector3 pos = new Vector3(xPos + bounds.extents.x / 2.0f, yPos, zPos + bounds.extents.z / 2.0f);
                    Instantiate(obstaclePrefab, pos, obstaclePrefab.transform.rotation, this.transform);
                }
            }
        }
    }

    public void RemoveObstacles(GameObject groundTile)
    {

    }
}
