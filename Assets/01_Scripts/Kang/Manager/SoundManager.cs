using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Clips
{
    public AudioClip button;
}

[Serializable]
public struct Clips3D
{
    public AudioClip bomb;
    public AudioClip Hit;
}

public class SoundManager : SingleTon<SoundManager>
{
    [Header("Sound")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource buttonSource;

    public Clips3D clips3D;
    public Clips clips;
    [HideInInspector] public bool loopPlaying = false;

    public void PlayAudio(AudioClip clip, float volumn = 1f)
    {
        sfxSource.PlayOneShot(clip, volumn);
    }
    public void PlayButton(float volumn)
    {
        buttonSource.PlayOneShot(clips.button, volumn);
    }
    public void Pause()
    {
        bgmSource.Pause();
        sfxSource.Pause();
    }
    public void Unpause()
    {
        bgmSource.UnPause();
        sfxSource.UnPause();
    }
}
