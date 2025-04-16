using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class FishingServerCommunicator : MonoBehaviour
{
    private const string SERVER_URL = "http://localhost:3000/api/fish/";
    
    public static FishingServerCommunicator Instance { get; private set; }
    
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
    
    public void StartFishing(Action<string, float> onSuccess, Action<string> onError)
    {
        StartCoroutine(StartFishingCoroutine(onSuccess, onError));
    }
    
    private IEnumerator StartFishingCoroutine(Action<string, float> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{SERVER_URL}start", ""))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<StartFishingResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response.guid, response.time / 1000f); // 밀리초 -> 초 변환
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
        var requestData = new EndFishingRequest
        {
            guid = guid,
            suc = success
        };
        
        string json = JsonConvert.SerializeObject(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        
        using (UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}end", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject(request.downloadHandler.text);
                
                if (response.suc)
                {
                    onSuccess?.Invoke(response.fish);
                }
                else
                {
                    onSuccess?.Invoke(null); // 실패한 경우
                }
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    // 데이터 클래스들
    [Serializable]
    private class StartFishingResponse
    {
        public string guid;
        public float time;
    }
    
    [Serializable]
    private class EndFishingRequest
    {
        public string guid;
        public bool suc;
    }
    
    [Serializable]
    public class EndFishingResponse
    {
        public bool suc;
        public FishJson fish;
    }
    
    [Serializable]
    public class FishJson
    {
        public string name;
        public string spec;
        public string rarity;
        public string trait;
        public string visualAddress;
        public string spriteAddress;
        public string id;
        public string description;
        public float weight;
        public float price;
        public float dancingStep;
    }
}