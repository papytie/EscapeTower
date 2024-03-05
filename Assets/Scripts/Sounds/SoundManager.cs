using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour {

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    private List<AudioClip> currentPlaylist = new();

    private void Awake() {
        if(musicAudioSource == null)
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        if(sfxAudioSource == null)
            sfxAudioSource = gameObject.AddComponent<AudioSource>();

        Random.InitState(DateTime.Now.Millisecond);
    }


    private void Update() {
        if(currentPlaylist.Count == 0) {
            currentPlaylist.AddRange(Game.GameSettings.Music.playlist);
        }

        if(!musicAudioSource.isPlaying) {
            int randomIndex = Random.Range(0, currentPlaylist.Count);
            PlayMusic(currentPlaylist[randomIndex]);
            currentPlaylist.RemoveAt(randomIndex);
        }
    }


    public void PlayMusic(AudioClip music) {
        musicAudioSource.clip = music;
        musicAudioSource.Play();
    }

    public void PauseMusic(bool paused) {
        if(paused) {
            musicAudioSource.Pause();
        } else {
            musicAudioSource.UnPause();
        }
    }

    public void PlaySoundEffect(AudioClip soundEffect) {
        sfxAudioSource.PlayOneShot(soundEffect);
    }
}
