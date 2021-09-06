using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }
    public void AdjustVolume(float volume)
    {
        audioSource.volume = volume;
    }
    public void StopPlaying()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

}
