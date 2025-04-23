// FishingServerConnector.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class FishingServerConnector : MonoBehaviour
{
    private const string SERVER_URL = "http://localhost:3000/api/";
    
    public static FishingServerConnector Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartFishing(Action<string, float, float> onSuccess, Action<string> onError)
    {
        StartCoroutine(StartFishingCoroutine(onSuccess, onError));
    }
    
    private IEnumerator StartFishingCoroutine(Action<string, float, float> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{SERVER_URL}fish/start", ""))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<StartFishingResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response.guid, response.time / 1000f, response.dancingStep);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    public void EndFishing(string guid, bool success, Action<FishJson> onSuccess, Action<string> onError)
    {
        StartCoroutine(EndFishingCoroutine(guid, success, onSuccess, onError));
    }
    
    private IEnumerator EndFishingCoroutine(string guid, bool success, Action<FishJson> onSuccess, Action<string> onError)
    {
        var requestData = new EndFishingRequest { guid = guid, suc = success };
        string json = JsonConvert.SerializeObject(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        
        using (UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}fish/end", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<EndFishingResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response.suc ? response.fish : null);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    [Serializable] private class StartFishingResponse { public string guid; public float time; public float dancingStep; }
    [Serializable] private class EndFishingRequest { public string guid; public bool suc; }
    [Serializable] public class EndFishingResponse { public bool suc; public FishJson fish; }
    [Serializable] public class getDataInventory { public fishesJson fishes; }
    [Serializable] public class dataReq { public string userId; }

    public void GetData(string userid, Action<FishJson[]> onSuccess)
    {
        StartCoroutine(GetDataCoroutine(userid, onSuccess));
    }

    private IEnumerator GetDataCoroutine(string userid, Action<FishJson[]> onSuccess)
    {
        var requestData = new dataReq { userId = userid };
        string json = JsonConvert.SerializeObject(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}datastore/fishtank", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            

            yield return request.SendWebRequest();
            Debug.Log(request.result);
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                FishJson[] response = JsonConvert.DeserializeObject<FishJson[]>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
        }
    }
}

[Serializable] public class fishesJson
{
    public FishJson[] fishes;
}

[Serializable]
public class FishJson
{
    public string name;
    public string spec;
    public string rarity;
    public string trait;
    public float purity;
    public string visualAddress;
    public string id;
    public string description;
    public float weight;
    public float price;
    public ItemType type;
}