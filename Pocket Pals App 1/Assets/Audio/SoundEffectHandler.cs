using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SoundEffectHandler : MonoBehaviour {

    public List<NamedAudio> audioList;
    public AudioSource audioSource;

    public void PlaySound(string key)
    {
        AudioClip clip = GetAudioSource(key);
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.Play();
        
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
