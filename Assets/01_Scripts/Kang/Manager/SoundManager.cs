using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Clips
{
    public AudioClip button;
    public AudioClip endLearn;
    public AudioClip jump;
    public AudioClip reload;
    public AudioClip gun;
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
    public AudioClip inGameBGM;
    public AudioClip mainBGM;
    public AudioClip learnBGM;
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public Clips3D clips3D;
    public Clips clips;
    [HideInInspector] public bool loopPlaying = false;

    public void PlayAudio(AudioClip clip, float volumn = 1f)
    {
        sfxSource.PlayOneShot(clip, volumn);
    }
    public void PlayButton(float volumn)
    {
        PlayAudio(clips.button, volumn);
    }
    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }
    public void ChangeSmooth(AudioClip clip)
    {
        bgmSource.DOFade(0f, 0.1f).OnComplete(() =>
        {
            bgmSource.Stop();
            bgmSource.volume = 0.1f;
            bgmSource.clip = clip;
            bgmSource.Play();
        });
    }
    public void StopSmooth()
    {
        bgmSource.DOFade(0f, 0.1f).OnComplete(() =>
        {
            bgmSource.Stop();
        });
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
