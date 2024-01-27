using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public SoundClip[] musicSound, sfxSound;
    public SoundClip[] footstepSounds;
    public AudioSource musicSource, playerSFXSource;

    private void Awake()
    {
        instance = this;
    }

    public void PlayMusic(string name)
    {
        SoundClip s = Array.Find(musicSound, x => x.name == name);
        if (s == null)
        {
            print("No Sound");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlayPlayerSFX(string name)
    {
        SoundClip s = Array.Find(sfxSound, x => x.name == name);
        if (s == null)
        {
            print("No Sound");
        }
        else
        {
            playerSFXSource.PlayOneShot(s.clip);
        }
    }

    public void PlayFootStepsSFX()
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Length);
            playerSFXSource.PlayOneShot(footstepSounds[randomIndex].clip);
        }
        else
        {
            print("No Footstep Sounds found");
        }
    }
}
