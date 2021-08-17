using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum MusicState
{
    playing,
    stop,
    pause
}

public class MusicManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioClip[] gameMusic;
    public AudioClip[] menuMusic;

    private AudioClip[] music;
    private MusicState state = MusicState.stop;
    private int currentTrackIndex = 0;
    private AudioSource audioSource;


    private void Update()
    {
        // play next track
        if (!audioSource.isPlaying && state == MusicState.playing)
        {
            PlayNextTrack();
        }   
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // initial settings
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioMixer.SetFloat("MusicHighpassCutoff", 10);

        PlayTrackByIndex(0);
    }

    public void SwitchMusicMode (SceneSwitchEventParam param)
    {
        if (param.state != SceneSwitchEventParam.SceneLoadStateEnum.LOADED) return;

        // when main menu loaded
        if (param.scene == SceneEnum.TITLE)
        {
            music = menuMusic;
            Stop();
            PlayNextTrack();
        }

        // from main menu to game
        if (param.scene == SceneEnum.GAME && param.previousScene != SceneEnum.GAME)
        {
            music = gameMusic;
            Stop();
            PlayNextTrack();
        }
    }

    public void SwitchMusicModeInGame (MenuToggleEventParam param)
    {
        // from game to pause menu
        if (param.showMenu)
        {
            audioMixer.SetFloat("MusicHighpassCutoff", 5000);
        }
        // from pause menu to game
        if (!param.showMenu)
        {
            audioMixer.SetFloat("MusicHighpassCutoff", 10);
        }
    }

    public void PlayNextTrack()
    {
        currentTrackIndex++;
        if (currentTrackIndex > music.Length - 1)
        {
            currentTrackIndex = 0;
        }
        PlayTrackByIndex(currentTrackIndex);
    }

    public void PlayTrackByName(string name)
    {
        AudioClip track = null;
        for (int i = 0; i < music.Length; i++)
        {
            if (music[i].name == name)
            {
                track = music[i];
                currentTrackIndex = i;
                break;
            }
        }

        if (track == null)
        {
            Debug.Log("Track with this name was not found");
            return;
        }
        else if (track != null)
        {
            Debug.Log("Playing music track: " + track.name);

            audioSource.clip = track;
            audioSource.Play();
            state = MusicState.playing;
        }
    }

    public void PlayTrackByIndex(int index)
    {
        audioSource.clip = music[index];
        audioSource.Play();
        state = MusicState.playing;
    }

    public void Pause()
    {
        audioSource.Pause();
        state = MusicState.pause;
    }

    public void Stop()
    {
        audioSource.Stop();
        state = MusicState.stop;
    }

    public string GetCurrentTrackName()
    {
        if (audioSource.clip == null) return null;
        return audioSource.clip.name;
    }
}
