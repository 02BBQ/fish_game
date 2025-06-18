using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SettingData
{
    public float _SFX;
    public float _BGM;
    public int _resolution;
    public int _fullScreen;
    public float _sensitivity;
    public bool _oceanRef;
    public bool _worldBend;
}

public class JsonManager : SingleTon<JsonManager>
{
    string _settingFile;

    private SettingData _settingData;

    //프로퍼티로 외부에서 변경하면 바로 저장
    public float SFX { get { return _settingData._SFX; } set { _settingData._SFX = value; SaveData(_settingData, _settingFile); } }
    public float BGM { get { return _settingData._BGM; } set { _settingData._BGM = value; SaveData(_settingData, _settingFile); } }
    public int Resolution { get { return _settingData._resolution; } set { _settingData._resolution = value; SaveData(_settingData, _settingFile); } }
    public int FullScreen { get { return _settingData._fullScreen; } set { _settingData._fullScreen = value; SaveData(_settingData, _settingFile); } }
    public float Sensitivity { get { return _settingData._sensitivity; } set { _settingData._sensitivity = value; SaveData(_settingData, _settingFile); } }
    public bool OceanRef { get { return _settingData._oceanRef; } set { _settingData._oceanRef = value; SaveData(_settingData, _settingFile); } }
    public bool WorldBend { get { return _settingData._worldBend; } set { _settingData._worldBend = value; SaveData(_settingData, _settingFile); } }

    [HideInInspector] public bool dataIsNull = false;

    private void Awake()
    {
        _settingFile = Path.Combine(Application.persistentDataPath + "/SettingData.json");


        if (File.Exists(_settingFile))
        {
            _settingData = LoadData<SettingData>(_settingFile);
        }
        else
        {
            ResetSettingData();
        }
    }
    private void ResetSettingData()
    {
        _settingData = new SettingData { _BGM = 1.401642f, _SFX = 1.401642f, _sensitivity = 4f, _oceanRef = true};
        SaveData(_settingData, _settingFile);
    }
    public void SaveData<T>(T data, string fileName) where T : class
    {
        if (File.Exists(fileName))
            File.Delete(fileName);

        string json = JsonUtility.ToJson(data);

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        string encodedJson = Convert.ToBase64String(bytes);

        File.WriteAllText(fileName, encodedJson);
    }

    public T LoadData<T>(string fileName) where T : class
    {
        if (File.Exists(fileName))
        {
            string jsonFromFile = File.ReadAllText(fileName);

            byte[] bytes = Convert.FromBase64String(jsonFromFile);
            string decodedJson = System.Text.Encoding.UTF8.GetString(bytes);

            return JsonUtility.FromJson<T>(decodedJson);
        }
        return null;
    }
}
