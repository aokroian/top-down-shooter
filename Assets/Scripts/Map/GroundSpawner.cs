using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundPrefab;
    public GameObject[] preCreatedGrounds { get; private set; }

    public float groundSize { get; private set; }

    private Dictionary<Vector3, GameObject> activeGround = new Dictionary<Vector3, GameObject>();
    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        groundSize = groundPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z * groundPrefab.transform.localScale.z;
        preCreatedGrounds = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject; ;
            preCreatedGrounds[i] = obj;
            activeGround.Add(obj.transform.position, obj);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnGround(Vector3 pos)
    {
        GameObject ground;
        if (pool.Count > 0)
        {
            ground = pool[0];
            pool.RemoveAt(0);
            ground.transform.position = pos;
        } else
        {
            ground = Instantiate(groundPrefab, pos, groundPrefab.transform.rotation, this.transform);
        }
        activeGround.Add(pos, ground);
        return ground;
    }

    public void RemoveGround(Vector3 pos)
    {
        GameObject ground = activeGround[pos];
        activeGround.Remove(pos);
        pool.Add(ground);
    }

    public GameObject GetGroundAt(Vector3 pos)
    {
        return activeGround[pos];
    }

    public bool HasGroundAt(Vector3 pos)
    {
        return activeGround.ContainsKey(pos);
    }
}
