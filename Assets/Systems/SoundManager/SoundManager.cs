using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private List<AudioClip> backgroundMusic;
    [SerializeField] private List<AudioClip> soundEffects;

    private Dictionary<string, AudioClip> sfxDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxDictionary = new Dictionary<string, AudioClip>();
            foreach (var clip in soundEffects)
            {
                sfxDictionary[clip.name] = clip;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name)
    {
        AudioClip musicClip = backgroundMusic.Find(clip => clip.name == name);
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip {name} not found!");
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxDictionary.TryGetValue(name, out AudioClip sfxClip))
        {
            sfxSource.PlayOneShot(sfxClip);
        }
        else
        {
            Debug.LogWarning($"Sound effect {name} not found!");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
