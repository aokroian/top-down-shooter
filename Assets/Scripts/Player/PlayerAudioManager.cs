using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{

    private AudioSource damageAudioSource;
    private AudioSource movementAudioSource;
    private AudioClip deathSound;
    private AudioClip runSound;
    private AudioClip dodgeSound;
    private AudioClip areaAttackSound;
    private AudioClip bulletHittingPlayerSound;
    private AudioClip sawHittingPlayerSound;
    private AudioClip switchWeaponSound;

    private void Start()
    {
        damageAudioSource = transform.Find("DamageAudioSource").GetComponent<AudioSource>();
        movementAudioSource = transform.Find("MovementAudioSource").GetComponent<AudioSource>();
        SerializableDictionary<string, AudioClip> audioStorage = GetComponent<AudioStorage>().audioDictionary;

        audioStorage.TryGetValue("Death", out deathSound);
        audioStorage.TryGetValue("Run", out runSound);
        audioStorage.TryGetValue("Dodge", out dodgeSound);
        audioStorage.TryGetValue("Hit by an enemy bullet", out bulletHittingPlayerSound);
        audioStorage.TryGetValue("Hit by an enemy saw", out sawHittingPlayerSound);
        audioStorage.TryGetValue("Switch weapon", out switchWeaponSound);
        audioStorage.TryGetValue("Area attack", out areaAttackSound);
    }
    public void PlaySwitchWeaponSound()
    {
        if (GameLoopController.paused)
        {
            damageAudioSource.clip = null;
            damageAudioSource.loop = false;
            damageAudioSource.pitch = 1;
            damageAudioSource.volume = 1;
            damageAudioSource.Stop();
            return;
        }
        damageAudioSource.PlayOneShot(switchWeaponSound);
    }


    // if arg is true - turning on
    // if arg is false - turning off
    public void PlayStepsSound(bool playOrStop)
    {
        if (GameLoopController.paused)
        {
            movementAudioSource.clip = null;
            movementAudioSource.loop = false;
            movementAudioSource.pitch = 1;
            movementAudioSource.volume = 1;
            movementAudioSource.Stop();
            return;
        }
        if (playOrStop)
        {
            movementAudioSource.clip = runSound;
            movementAudioSource.loop = true;
            movementAudioSource.pitch = Random.Range(0.6f, 0.8f);
            movementAudioSource.volume = Random.Range(0.8f, 1f);
            if (!movementAudioSource.isPlaying)
            {
                movementAudioSource.Play();
            }
        }
        else if (movementAudioSource.clip == runSound && movementAudioSource.isPlaying && !playOrStop)
        {
            movementAudioSource.clip = null;
            movementAudioSource.loop = false;
            movementAudioSource.pitch = 1;
            movementAudioSource.volume = 1;
            movementAudioSource.Stop();
        }
    }

    public void PlayAreaAttackSound(bool enabled)
    {
        if (GameLoopController.paused)
        {
            damageAudioSource.clip = null;
            damageAudioSource.loop = false;
            damageAudioSource.pitch = 1;
            damageAudioSource.volume = 1;
            damageAudioSource.Stop();
            return;
        }
        if (enabled)
        {
            damageAudioSource.clip = areaAttackSound;
            damageAudioSource.loop = true;
            damageAudioSource.pitch = Random.Range(0.6f, 0.8f);
            damageAudioSource.volume = Random.Range(0.8f, 1f);
            if (!damageAudioSource.isPlaying)
            {
                damageAudioSource.Play();
            }
        }
        else if (damageAudioSource.clip == areaAttackSound && damageAudioSource.isPlaying && !enabled)
        {
            damageAudioSource.clip = null;
            damageAudioSource.loop = false;
            damageAudioSource.pitch = 1;
            damageAudioSource.volume = 1;
            damageAudioSource.Stop();
        }
    }

    public void PlayHitByEnemySawSound()
    {
        if (GameLoopController.paused)
        {
            damageAudioSource.clip = null;
            damageAudioSource.loop = false;
            damageAudioSource.pitch = 1;
            damageAudioSource.volume = 1;
            damageAudioSource.Stop();
            return;
        }
        damageAudioSource.PlayOneShot(sawHittingPlayerSound);
    }
    public void PlayHitByEnemyBulletSound()
    {
        if (GameLoopController.paused)
        {
            damageAudioSource.clip = null;
            damageAudioSource.loop = false;
            damageAudioSource.pitch = 1;
            damageAudioSource.volume = 1;
            damageAudioSource.Stop();
            return;
        }
        damageAudioSource.PlayOneShot(bulletHittingPlayerSound);
    }
    public void PlayDeathSound()
    {

        damageAudioSource.PlayOneShot(deathSound);
    }
    public void PlayDodgeSound()
    {
        movementAudioSource.PlayOneShot(dodgeSound);
    }
}
