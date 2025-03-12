using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class SettingManager : SingleTon<SettingManager>
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _BGMSlider;
    [SerializeField] private Slider _SFXSlider;
    [SerializeField] private Slider _sensSlider;
    //[SerializeField] private RotateCamera _rotCam;

    private void Start()
    {
        Apply();
    }
    public void Apply()
    {
        _BGMSlider.value = JsonManager.Instance.BGM;
        _SFXSlider.value = JsonManager.Instance.SFX;
        _sensSlider.value = JsonManager.Instance.Sensitivity;
        _audioMixer.SetFloat("BGM", Mathf.Log10(_BGMSlider.value) * 20f);
        _audioMixer.SetFloat("SFX", Mathf.Log10(_SFXSlider.value) * 20f);
        //_rotCam.sens = _sensSlider.value;
    }

    public void SetBGMVolume(float volume)
    {
        _audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20f);
        JsonManager.Instance.BGM = volume;
    }

    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20f);
        JsonManager.Instance.SFX = volume;
    }
    //public void SetSensitivity(float volume)
    //{
    //    _rotCam.sens = volume;
    //    JsonManager.Instance.Sensitivity = volume;
    //}
}