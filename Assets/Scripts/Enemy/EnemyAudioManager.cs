using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAudioManager : MonoBehaviour
{
    public float maxWalkSoundDistance = 10;
    private Transform player;
    private NavMeshAgent agent;

    // main for damage etc
    private AudioSource mainAudioSource;
    // movement for steps
    private AudioSource movementAudioSource;

    private AudioClip walkSound;
    private AudioClip getHitByPlayer;
    private AudioClip aimingSound;
    // saw robot only
    private AudioClip sawSound;
    // kamikaze robot only
    private AudioClip soundBeforeSuicide;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        mainAudioSource = transform.Find("MainAudioSource")?.GetComponent<AudioSource>();
        movementAudioSource = transform.Find("MovementAudioSource")?.GetComponent<AudioSource>();
        SerializableDictionary<string, AudioClip> audioStorage = GetComponent<AudioStorage>().audioDictionary;

        audioStorage.TryGetValue("Walk", out walkSound);
        audioStorage.TryGetValue("Hit by player bullet", out getHitByPlayer);
        audioStorage.TryGetValue("Aiming", out aimingSound);
        audioStorage.TryGetValue("Saw", out sawSound);
        audioStorage.TryGetValue("Before suicide", out soundBeforeSuicide);
    }


    public void PlayMovementSound(bool isPlaying)
    {
        if (walkSound == null)
        {
            Debug.Log("No walking sound found in enemy audio manager " + gameObject.name);
            return;
        }

        if (player == null)
        {
            Debug.Log("Player not found in enemy audio manager " + gameObject.name);
            return;
        }
        if (agent.velocity.magnitude <= 0)
        {
            movementAudioSource.clip = null;
            movementAudioSource.loop = false;
            movementAudioSource.pitch = 1;
            movementAudioSource.volume = 1;
            movementAudioSource.Stop();
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (distToPlayer >= maxWalkSoundDistance)
        {
            return;
        }

        float volume = distToPlayer / maxWalkSoundDistance;

        if (isPlaying)
        {  
            if (!movementAudioSource.isPlaying)
            {
                movementAudioSource.clip = walkSound;
                movementAudioSource.loop = true;
                movementAudioSource.pitch = Random.Range(0.6f, 0.8f);
                movementAudioSource.volume = Random.Range(0.8f, 1f) * volume;
                movementAudioSource.Play();
            }
        }
        else
        {
            movementAudioSource.clip = null;
            movementAudioSource.loop = false;
            movementAudioSource.pitch = 1;
            movementAudioSource.volume = 1;
            movementAudioSource.Stop();
        }
    }

    public void PlayAimingSound(bool isPlaying)
    {
        if (aimingSound == null)
        {
            Debug.Log("No aiming sound found in enemy " + gameObject.name);
            return;
        }

        if (isPlaying)
        {
            mainAudioSource.clip = aimingSound;
            mainAudioSource.loop = true;
            mainAudioSource.pitch = 1;
            mainAudioSource.volume = 1;
            if (!mainAudioSource.isPlaying)
            {
                mainAudioSource.Play();
            }
        }
        else
        {
            mainAudioSource.clip = null;
            mainAudioSource.loop = false;
            mainAudioSource.pitch = 1;
            mainAudioSource.volume = 1;
            mainAudioSource.Stop();
        }
    }

    public void PlayHitByPlayerBulletSound()
    {
        if (getHitByPlayer == null)
        {
            Debug.Log("No get hit by player sound sound found in enemy " + gameObject.name);
            return;
        }

        mainAudioSource.PlayOneShot(getHitByPlayer);
    }

    public void PlaySawSound(bool isPlaying)
    {
        if (sawSound == null)
        {
            Debug.Log("No saw sound found in enemy " + gameObject.name);
            return;
        }

        if (isPlaying)
        {
            mainAudioSource.clip = sawSound;
            mainAudioSource.loop = true;
            mainAudioSource.pitch = 1;
            mainAudioSource.volume = 1;
            if (!mainAudioSource.isPlaying)
            {
                mainAudioSource.Play();
            }
        }
        else
        {
            mainAudioSource.clip = null;
            mainAudioSource.loop = false;
            mainAudioSource.pitch = 1;
            mainAudioSource.volume = 1;
            mainAudioSource.Stop();
        }
    }

    public void PlaySoundBeforeSuicide()
    {
        if (soundBeforeSuicide == null)
        {
            Debug.Log("No saw sound before suicide found in enemy " + gameObject.name);
            return;
        }
        mainAudioSource.PlayOneShot(soundBeforeSuicide);
    }
}
