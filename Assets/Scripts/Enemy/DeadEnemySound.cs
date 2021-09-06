using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemySound : MonoBehaviour
{
    public float maxDistance = 15;
    public float minVolume = 0.1f;

    private AudioSource mainAudioSource;
    private AudioClip deathExplosionSound;
    private Transform playerObj;
    // Start is called before the first frame update
    void Start()
    {
        mainAudioSource = transform.Find("MainAudioSource")?.GetComponent<AudioSource>();
        SerializableDictionary<string, AudioClip> audioStorage = GetComponent<AudioStorage>().audioDictionary;
        playerObj = GameObject.Find("Player").transform;
        float distance = Vector3.Distance(playerObj.position, transform.position);
        float vol = (maxDistance - distance) / maxDistance;
        if (vol < minVolume)
        {
            vol = minVolume;
        }
        audioStorage.TryGetValue("Death", out deathExplosionSound);
        if (distance <= maxDistance)
        {
            PlayDeathSound(vol) ;
        }
    }

    public void PlayDeathSound(float volume)
    {
        if (GameLoopController.paused)
        {
            return;
        }
        if (deathExplosionSound == null)
        {
            Debug.Log("No death explosion sound sound found in enemy " + gameObject.name);
            return;
        }
        mainAudioSource.clip = deathExplosionSound;
        mainAudioSource.loop = false;
        mainAudioSource.pitch = 1;
        mainAudioSource.volume = volume;
        mainAudioSource.Play();
    }
}
