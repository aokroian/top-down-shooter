using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLavaController : MonoBehaviour
{
    public float expandSpeed = 1;
    public float outerRadiusOffset = 2f;
    public GameObject player;
    public GameObject enemies;
    public GameObject obstacles;
    public float playerDamage = 5;
    public float enemyDamage = 1;

    //sound
    public float maxSoundDistance = 15;
    private LavaSoundManager lavaSoundManager;

    public float currentRadius { get; private set; }
    private Material material;

    private PlayerController playerController;
    private EnvironmentLavaController environmentLavaController;

    private void Start()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        playerController = player.GetComponent<PlayerController>();
        environmentLavaController = obstacles.GetComponent<EnvironmentLavaController>();
        lavaSoundManager = GetComponent<LavaSoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        currentRadius += expandSpeed * Time.deltaTime;
        material.SetFloat("OuterRadius", currentRadius + outerRadiusOffset);
        Burn();

        // sound
        float playerFromStartDist = player.GetComponent<PlayerController>().distance;
        float playerFromLavaDist = playerFromStartDist - currentRadius;
        if (playerFromLavaDist < 0) playerFromLavaDist = 0;
        float volume = (maxSoundDistance - playerFromLavaDist) / maxSoundDistance;
        lavaSoundManager.AdjustVolume(volume);
    }

    private void Burn()
    {
        BurnPlayer();
        BurnEnemies();
        BurnObstacles();
    }

    private void BurnPlayer()
    {
        if (playerController.distance < currentRadius)
        {
            player.GetComponent<Target>().TakeDamage(playerDamage);
        }
    }

    private void BurnEnemies()
    {
        StartCoroutine(DamageEnemiesCoroutine());
    }

    private IEnumerator DamageEnemiesCoroutine()
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

    private void BurnObstacles()
    {
        environmentLavaController.Burn(currentRadius);
    }
    
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(Vector3.zero, Time.time * expandSpeed - 2);
    }
    */
}
