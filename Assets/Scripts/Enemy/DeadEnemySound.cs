using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemySound : MonoBehaviour
{
    private AudioSource mainAudioSource;
    private AudioClip deathExplosionSound;
    // Start is called before the first frame update
    void Start()
    {
        mainAudioSource = transform.Find("MainAudioSource")?.GetComponent<AudioSource>();
        SerializableDictionary<string, AudioClip> audioStorage = GetComponent<AudioStorage>().audioDictionary;

        audioStorage.TryGetValue("Death", out deathExplosionSound);
        PlayDeathSound();
    }

    public void PlayDeathSound()
    {
        if (deathExplosionSound == null)
        {
            Debug.Log("No death explosion sound sound found in enemy " + gameObject.name);
            return;
        }
        mainAudioSource.clip = deathExplosionSound;
        mainAudioSource.loop = false;
        mainAudioSource.pitch = 1;
        mainAudioSource.volume = 1;
        mainAudioSource.Play();
    }
}
