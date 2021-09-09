using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLavaController : MonoBehaviour
{
    public float expandSpeed = 1;
    public float outerRadiusOffset = 2f;
    public float speedUpTime = 5f;
    public float maxPlayerDistance = 50f;
    public GameObject player;
    public GameObject enemies;
    public GameObject obstacles;
    public GameObject environment;
    public float playerDamage = 5;
    public float playerDamageInterval = 1f;
    public float enemyDamage = 1;

    //sound
    public float maxSoundDistance = 15;
    private LavaSoundManager lavaSoundManager;

    public float currentRadius { get; private set; }
    private Material material;
    private float timeFromStart;

    private PlayerController playerController;
    private EnvironmentLavaController environmentLavaController;

    private IEnumerator playerDamageCoroutine;

    private void Start()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        playerController = player.GetComponent<PlayerController>();
        environmentLavaController = obstacles.GetComponent<EnvironmentLavaController>();
        for(int i = 0; i < environment.transform.childCount; i ++)
        {
            environmentLavaController.ObstacleSpawned(environment.transform.GetChild(i).gameObject);
        }
        lavaSoundManager = GetComponent<LavaSoundManager>();

        StartBurnEnemies();
    }

    void Update()
    {
        Expand();

        // sound
        float playerFromStartDist = playerController.distance;
        float playerFromLavaDist = playerFromStartDist - currentRadius;
        if (playerFromLavaDist < 0) playerFromLavaDist = 0;
        float volume = (maxSoundDistance - playerFromLavaDist) / maxSoundDistance;
        
        if (GameLoopController.paused)
        {

            lavaSoundManager.TogglePlaying(false);
        }
        else if (!GameLoopController.paused)
        {
            lavaSoundManager.AdjustVolume(volume);
            lavaSoundManager.TogglePlaying(true);
        }
    }

    private void LateUpdate()
    {
        Burn();
    }

    private void Expand()
    {
        if (playerController.distance - currentRadius < maxPlayerDistance)
        {
            if (timeFromStart < speedUpTime) {
                timeFromStart += Time.deltaTime;
                var speedUpMultiplier = timeFromStart / speedUpTime;
                currentRadius += expandSpeed * Time.deltaTime * speedUpMultiplier;
            } else
            {
                currentRadius += expandSpeed * Time.deltaTime;
            }
        } else {
            currentRadius = playerController.distance - maxPlayerDistance;
        }
        material.SetFloat("OuterRadius", currentRadius + outerRadiusOffset);
    }

    private void Burn()
    {
        BurnPlayer();
        
        BurnObstacles();
    }

    private void BurnPlayer()
    {
        if (playerDamageCoroutine == null && playerController.distance < currentRadius)
        {

            playerDamageCoroutine = DamagePlayerCoroutine();
            StartCoroutine(playerDamageCoroutine);
        } else if (playerDamageCoroutine != null && playerController.distance >= currentRadius)
        {
            StopCoroutine(playerDamageCoroutine);
            playerDamageCoroutine = null;
        }
    }

    private IEnumerator DamagePlayerCoroutine()
    {
        while (true)
        {
            player.GetComponent<Target>().TakeDamage(playerDamage);
            yield return new WaitForSeconds(playerDamageInterval);
        }
    }

    private void StartBurnEnemies()
    {
        StartCoroutine(DamageEnemiesCoroutine());
    }

    private IEnumerator DamageEnemiesCoroutine()
    {
        while (true)
        {
            foreach (Target enemy in enemies.GetComponentsInChildren<Target>())
            {
                if (Vector3.Distance(Vector3.zero, enemy.transform.position) < currentRadius)
                {
                    enemy.TakeDamage(enemyDamage);
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void BurnObstacles()
    {
        environmentLavaController.Burn(currentRadius);
    }

    public void ShowElectricity(float radius)
    {
        material.SetVector("PlayerPos", new Vector2(player.transform.position.x, player.transform.position.z));

        // 2x, dont know why!
        material.SetFloat("LightingRadius", radius * 2);
    }

    public void HideElectricity() {
        material.SetFloat("LightingRadius", 0f);
    }
    
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(Vector3.zero, Time.time * expandSpeed - 2);
    }
    */
}
