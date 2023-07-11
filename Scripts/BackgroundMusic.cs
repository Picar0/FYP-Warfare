using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip[] musicClips;
    private AudioSource audioSource;

    private int currentClipIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayMusic();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentClipIndex++;
            if (currentClipIndex >= musicClips.Length)
            {
                currentClipIndex = 0;
            }
            PlayMusic();
        }
    }

    void PlayMusic()
    {
        audioSource.clip = musicClips[currentClipIndex];
        audioSource.loop = false;
        audioSource.Play();
    }
}
