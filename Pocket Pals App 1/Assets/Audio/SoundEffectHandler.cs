﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SoundEffectHandler : MonoBehaviour {

    public static SoundEffectHandler Instance { set; get; }
    public List<NamedAudio> audioList;
    public List<NamedAudio> animalSounds;
    public AudioSource audioSource;

    private void Start()
    {
        Instance = this;
	}

    public void PlaySound(string key)
    {
        AudioClip clip = GetAudioSource(key);
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.Play();
        
    }

    public void PlayAnimalSound(string id)
    {
        AudioClip clip = GetPPalSound(id);
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.Play();

    }

    private AudioClip GetPPalSound(string id)
    {
        foreach (NamedAudio na in animalSounds)
        {
            if (id == na.name) return na.audio;
        }
        return null;
    }

    private AudioClip GetAudioSource(string s)
    {
        foreach (NamedAudio na in audioList)
        {
            if (s == na.name) return na.audio;
        }
        return null;
    }

}

[Serializable]
public struct NamedAudio
{
    public string name;
    public AudioClip audio;
}
