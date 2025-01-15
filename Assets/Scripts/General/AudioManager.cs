using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioClip[] clipCollection = new AudioClip[8];
    [SerializeField] AudioClip musicClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    public static AudioManager Instance { get; private set; } 

    private void Awake()
    {
        Instance = this;
        audioSource.spatialBlend = 0f;

        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        PlayMusic();
    }

    public void PlaySFX(int clipIndex, float volume = 1f)
    {
        if (clipCollection[clipIndex] != null)
            audioSource.PlayOneShot(clipCollection[clipIndex], volume);
    }

    private void PlayMusic()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }
}
