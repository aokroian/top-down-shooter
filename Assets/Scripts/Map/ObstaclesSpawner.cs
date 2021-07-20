using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ObstacleWrapper
    {
        public GameObject obstacle;
        public float yValue;
        public bool big;
    }

    //public GameObject obstaclePrefab;
    public ObstacleWrapper[] obstacles;
    //public float minY = 0.7f;
    public float noiseMultiplier = 1.11f;
    public float obstacleSize;

    [Range(0, 999999)]
    public int noiseSeed;
    public bool randomSeed = false;

    //private float obstacleSize;
    private float yPos;

    private float noiseOffset = 10000;

    private Dictionary<Vector2, List<GameObject>> activeObstacles = new Dictionary<Vector2, List<GameObject>>();
    private Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        if (randomSeed)
        {
            noiseSeed = Mathf.RoundToInt(Random.Range(0, 999999));
        }
        //obstacleSize = obstaclePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * obstaclePrefab.transform.localScale.z;
        //yPos = (obstaclePrefab.transform.position.y - obstaclePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.min.y) * obstaclePrefab.transform.localScale.z;

        yPos = 0f; // TODO: Remove?
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnObstacles(GameObject groundTile)
    {
        var bounds = groundTile.GetComponent<MeshRenderer>().bounds;
        //Debug.Log("Size: " + bounds.size.z);

        var maxCount = Mathf.Floor(bounds.size.z / obstacleSize);
        var iterStep = bounds.size.z / maxCount;

        var key = GetKeyByTile(groundTile);
        List<GameObject> tileObstacles;
        if (activeObstacles.ContainsKey(key)) {
            tileObstacles = activeObstacles[key];
        } else
        {
            tileObstacles = new List<GameObject>();
            activeObstacles.Add(key, tileObstacles);
        }

        HashSet<Vector2> occupied = new HashSet<Vector2>();

        for (int x = 0; x < maxCount; x ++)
        {
            var xPos = bounds.min.x + iterStep * x;
            for (int z = 0; z < maxCount; z ++)
            {
                if (occupied.Contains(new Vector2(x, z)))
                {
                    continue;
                }

                var zPos = bounds.min.z + iterStep * z;
                var y = Mathf.PerlinNoise(noiseOffset + xPos * noiseMultiplier, noiseOffset + zPos * noiseMultiplier);
                bool bigAcceptable = x < maxCount - 1 && z < maxCount - 1 && !occupied.Contains(new Vector2(x + 1, z)) && !occupied.Contains(new Vector2(x, z + 1));
                ObstacleWrapper wrapper = GetObstacleByValueOrNull(y, bigAcceptable);

                GameObject obstaclePrefab = wrapper.obstacle;
                if (obstaclePrefab != null)
                {
                    GameObject obstacle;
                    //Vector3 pos = new Vector3(xPos + bounds.extents.x / 2.0f, yPos, zPos + bounds.extents.z / 2.0f);
                    Vector3 pos;
                    Quaternion rotation;

                    if (wrapper.big)
                    {
                        bool horizontal = (int)(y * 10000) % 2 == 1;
                        if (horizontal)
                        {
                            var xPosNext = bounds.min.x + iterStep * (x + 1);
                            var delta = (xPosNext - xPos) / 2;
                            pos = new Vector3(xPos + delta, yPos, zPos);
                            rotation = Quaternion.Euler(0f, 90f, 0f);
                            occupied.Add(new Vector2(x + 1, z));
                        }
                        else
                        {
                            var zPosNext = bounds.min.z + iterStep * (z + 1);
                            var delta = (zPosNext - zPos) / 2;
                            pos = new Vector3(xPos, yPos, zPos + delta);
                            rotation = Quaternion.Euler(0f, 0f, 0f);
                            occupied.Add(new Vector2(x, z + 1));
                        }
                    } else
                    {
                        pos = new Vector3(xPos, yPos, zPos);
                        rotation = Quaternion.Euler(0f, 0f, 0f);
                        //rotation = obstaclePrefab.transform.rotation;
                    }


                    if (pool.ContainsKey(obstaclePrefab.tag) && pool[obstaclePrefab.tag].Count > 0)
                    {
                        obstacle = pool[obstaclePrefab.tag][0];
                        obstacle.transform.position = pos;
                        obstacle.transform.rotation = obstaclePrefab.transform.rotation * rotation;
                        pool[obstaclePrefab.tag].RemoveAt(0);
                    } else
                    {
                        obstacle = Instantiate(obstaclePrefab, pos, rotation * obstaclePrefab.transform.rotation, this.transform);
                    }

                    tileObstacles.Add(obstacle);
                }
            }
        }
    }

    public void RemoveObstacles(GameObject groundTile)
    {
        var key = GetKeyByTile(groundTile);
        if (activeObstacles.ContainsKey(key))
        {
            var obstacles = activeObstacles[key];
            foreach (GameObject obs in obstacles)
            {
                if (!pool.ContainsKey(obs.tag))
                {
                    pool.Add(obs.tag, new List<GameObject>());
                }
                pool[obs.tag].Add(obs);
            }
            activeObstacles.Remove(key);
        }
    }

    private Vector2 GetKeyByTile(GameObject groundTile)
    {
        return new Vector2(groundTile.transform.position.x, groundTile.transform.position.z);
    }

    private ObstacleWrapper GetObstacleByValueOrNull(float value, bool bigAcceptable)
    {
        ObstacleWrapper current = new ObstacleWrapper();

        foreach(ObstacleWrapper wrapper in obstacles)
        {
            if (value > wrapper.yValue && current.yValue < wrapper.yValue && (bigAcceptable || !wrapper.big))
            {
                current = wrapper;
            }
        }

        return current;
    }
}
