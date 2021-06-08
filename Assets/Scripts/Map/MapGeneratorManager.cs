using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(GroundSpawner), typeof(ObstaclesSpawner))]
public class MapGeneratorManager : MonoBehaviour
{
    public GameObject player;
    
    public int tilesCount = 2;

    public EnemySpawnerManager enemySpawner;

    private GroundSpawner groundSpawner;
    private ObstaclesSpawner obstaclesSpawner;

    private float groundSize;

    private Dictionary<Vector2, bool> grounds = new Dictionary<Vector2, bool>();

    private Vector2 curTilePos; // TODO: Мб не обязательно полем?

    // Start is called before the first frame update
    void Start()
    {
        groundSpawner = GetComponent<GroundSpawner>();
        obstaclesSpawner = GetComponent<ObstaclesSpawner>();
        groundSize = groundSpawner.groundSize;
        foreach (GameObject obj in groundSpawner.preCreatedGrounds)
        {
            Vector2 pos = ToTilePos(obj.transform.position);
            grounds.Add(pos, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 prevTilePos = curTilePos;
        CalcCurTilePos();

        if (prevTilePos != curTilePos)
        {
            CreateAndMarkActive();
            RemoveInactive();
            GetComponent<NavMeshSurface>().BuildNavMesh();
            enemySpawner.Spawn(groundSpawner.GetGroundAt(FromTilePos(curTilePos)));
        }
    }

    private void CreateAndMarkActive()
    {
        grounds.Keys.ToList().ForEach(k => grounds[k] = false);

        for (int x = (int)curTilePos.x - tilesCount; x <= (int)curTilePos.x + tilesCount; x++)
        {
            for (int y = (int)curTilePos.y - tilesCount; y <= (int)curTilePos.y + tilesCount; y++)
            {
                Vector2 pos = new Vector2(x, y);
                if (!grounds.ContainsKey(pos))
                {
                    var ground = groundSpawner.SpawnGround(FromTilePos(pos));
                    obstaclesSpawner.SpawnObstacles(ground);
                }
                grounds[pos] = true;
            }
        }
    }

    private void RemoveInactive()
    {
        foreach(Vector2 key in grounds.Keys.ToList())
        {
            if (grounds[key])
            {
                continue;
            }
            Vector3 pos = FromTilePos(key);
            if (groundSpawner.HasGroundAt(pos)) {
                obstaclesSpawner.RemoveObstacles(groundSpawner.GetGroundAt(pos));
                groundSpawner.RemoveGround(pos);
            }
            // TODO: DESPAWN???

            grounds.Remove(key);
        }
    }

    private void CalcCurTilePos()
    {
        curTilePos = ToTilePos(player.transform.position);
    }

    private Vector2 ToTilePos(Vector3 pos)
    {
        return new Vector2(Mathf.Round(pos.x / groundSize), Mathf.Round(pos.z / groundSize));
    }

    private Vector3 FromTilePos(Vector2 tilePos)
    {
        return new Vector3(tilePos.x * groundSize, 0.0f, tilePos.y * groundSize);
    }
}
