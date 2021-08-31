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
    public SettingsHolder settingsHolder;

    public string masterVolumeParamName;
    public string musicVolumeParamName;
    public string sfxVolumeParamName;

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
        music = menuMusic;
        audioSource = GetComponent<AudioSource>();

        // initial settings
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioMixer.SetFloat("MusicHighpassCutoff", 10);

        TogglePause(false);
        SettingsChanged();
    }

    public void SwitchMusicMode(SceneSwitchEventParam param)
    {
        if (param.state != SceneSwitchEventParam.SceneLoadStateEnum.LOADED) return;
        audioMixer.SetFloat("MusicHighpassCutoff", 10);

        // when main menu loaded
        if (param.scene == SceneEnum.TITLE && music != menuMusic)
        {
            music = menuMusic;
            Stop();
            // random track
            currentTrackIndex = Random.Range(0, music.Length - 1);
            PlayNextTrack();
        }

        // from main menu to game
        if (param.scene == SceneEnum.GAME && param.previousScene != SceneEnum.GAME)
        {
            music = gameMusic;
            Stop();
            // random track
            currentTrackIndex = Random.Range(0, music.Length - 1);
            PlayNextTrack();
        }
    }

    public void SwitchMusicModeInGame(MenuToggleEventParam param)
    {
        // from game to pause menu
        if (param.showMenu)
        {
            audioMixer.SetFloat("MusicHighpassCutoff", 3000);
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
        TogglePause(!settingsHolder.music);
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

            var prevTrack = audioSource.clip;
            audioSource.clip = track;
            audioSource.Play();
            state = MusicState.playing;

            if (prevTrack != null)
            {
                prevTrack.UnloadAudioData();
            }
        }
    }

    public void PlayTrackByIndex(int index)
    {
        var prevTrack = audioSource.clip;
        audioSource.clip = music[index];
        if (prevTrack != null)
        {
            prevTrack.UnloadAudioData();
        }
    }

    public void TogglePause(bool pause)
    {
        if (!pause)
        {
            audioSource.Play();
            state = MusicState.playing;
        }
        else
        {
            audioSource.Pause();
            state = MusicState.pause;
        }
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

    public void SettingsChanged()
    {
        bool playing = state == MusicState.playing;
        Debug.Log("mUSIC: " + settingsHolder.music + "; " + playing);
        if (playing != settingsHolder.music)
        {
            if (settingsHolder.music)
            {
                TogglePause(false);
                //audioMixer.SetFloat(musicVolumeParamName, 0);
            }
            else
            {
                TogglePause(true);
                //audioMixer.SetFloat(musicVolumeParamName, -80);
            }
        }
        audioMixer.SetFloat(sfxVolumeParamName, settingsHolder.sfx ? 0 : -80);
    }
}
