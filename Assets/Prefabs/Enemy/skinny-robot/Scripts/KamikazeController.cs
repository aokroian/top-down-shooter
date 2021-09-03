using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeController : MonoBehaviour
{
    public GameObject[] explosives;
    public float distanceForSound;

    private Transform player;
    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= distanceForSound)
        {
            GetComponent<EnemyAudioManager>().PlaySoundBeforeSuicide(true);
        } else
        {
            GetComponent<EnemyAudioManager>().PlaySoundBeforeSuicide(true);
        }
    }
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            CommitSuicide();
        }
    }

    public void CommitSuicide()
    {
        GetComponent<Target>().health -= 10000000000;
        //GetComponent<Destructible>().ChangeToDestructible();
        foreach (GameObject explosive in explosives)
        {
            explosive.GetComponent<Explosion>().Explode();
        }
    }
}
