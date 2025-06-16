using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SettingManager : SingleTon<SettingManager>
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _BGMSlider;
    [SerializeField] private Slider _SFXSlider;
    [SerializeField] private Slider _sensSlider;
    [SerializeField] private Material _ocean;
    [SerializeField] private Toggle _oceanReflection;
    [SerializeField] private Toggle _worldBend;
    public string materialLabel = "MyMaterials";

    List<Material> mats = new List<Material>();
    private void Start()
    {
        Apply();
    }
    public void Apply()
    {
        _BGMSlider.value = JsonManager.Instance.BGM;
        _SFXSlider.value = JsonManager.Instance.SFX;
        _worldBend.isOn = JsonManager.Instance.OceanRef;
        _worldBend.isOn = JsonManager.Instance.WorldBend;
        _sensSlider.value = JsonManager.Instance.Sensitivity;
        _audioMixer.SetFloat("BGM", Mathf.Log10(_BGMSlider.value) * 20f);
        _audioMixer.SetFloat("SFX", Mathf.Log10(_SFXSlider.value) * 20f);
        _ocean.SetFloat("_Ref", _worldBend.isOn ? 1f : 0f);
        LoadMaterialsByLabel(materialLabel);
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
    public void SetSensitivity(float volume)
    {
        //_rotCam.sens = volume;
        JsonManager.Instance.Sensitivity = volume;
    }
    public void SetOceanRef(bool value)
    {
        JsonManager.Instance.OceanRef = value;
        _ocean.SetFloat("_Ref", value ? 1f : 0f);
    }
    public void SetWorldBend(bool value)
    {
        JsonManager.Instance.WorldBend = value;
        ChangeBend(JsonManager.Instance.WorldBend);
    }


    void LoadMaterialsByLabel(string label)
    {
        Addressables.LoadAssetsAsync<Material>(label, null).Completed += OnMaterialsLoaded;
    }

    void OnMaterialsLoaded(AsyncOperationHandle<IList<Material>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<Material> materials = handle.Result;
            Debug.Log($"Loaded {materials.Count} materials.");
            foreach (Material mat in materials)
            {
                mats.Add(mat);
            }
            ChangeBend(JsonManager.Instance.WorldBend);
        }
        else
        {
            Debug.LogError("Failed to load materials.");
        }
    }
    void ChangeBend(bool value)
    {
        foreach (Material mat in mats)
        {
            mat.SetFloat("_Bend", JsonManager.Instance.WorldBend ? 1f : 0f);
        }
    }
}