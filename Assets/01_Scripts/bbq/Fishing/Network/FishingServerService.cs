using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace fishing.Network
{
    public class FishingServerService : MonoBehaviour, IFishingServerService
    {
        private IRetryPolicy _retryPolicy;
        
        private void Awake()
        {
            _retryPolicy = new ExponentialBackoffRetryPolicy(ServerConfig.MaxRetries, ServerConfig.RetryDelaySeconds);
        }
        
        public async Task<Result<StartFishingResponse>> StartFishing()
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var request = UnityWebRequest.PostWwwForm($"{ServerConfig.BaseUrl}fish/start", "");
                    await request.SendWebRequest();
                    
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var response = JsonConvert.DeserializeObject<StartFishingResponse>(request.downloadHandler.text);
                        return Result<StartFishingResponse>.Success(response);
                    }
                    
                    return Result<StartFishingResponse>.Failure(new Error(
                        $"Server error: {request.error}",
                        Error.ErrorType.Server
                    ));
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Fishing start failed: {ex}");
                return Result<StartFishingResponse>.Failure(new Error(
                    "Unexpected error occurred",
                    Error.ErrorType.Unknown,
                    ex
                ));
            }
        }
        
        public async Task<Result<FishJson>> EndFishing(string guid, bool success)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var requestData = new EndFishingRequest { guid = guid, suc = success };
                    string json = JsonConvert.SerializeObject(requestData);
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                    
                    using var request = new UnityWebRequest($"{ServerConfig.BaseUrl}fish/end", "POST");
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    
                    await request.SendWebRequest();
                    
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var response = JsonConvert.DeserializeObject<EndFishingResponse>(request.downloadHandler.text);
                        return Result<FishJson>.Success(response.suc ? response.fish : null);
                    }
                    
                    return Result<FishJson>.Failure(new Error(
                        $"Server error: {request.error}",
                        Error.ErrorType.Server
                    ));
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Fishing end failed: {ex}");
                return Result<FishJson>.Failure(new Error(
                    "Unexpected error occurred",
                    Error.ErrorType.Unknown,
                    ex
                ));
            }
        }
        
        public async Task<Result<InitData>> GetData(string userId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var requestData = new dataReq { userId = userId };
                    string json = JsonConvert.SerializeObject(requestData);
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                    
                    using var request = new UnityWebRequest($"{ServerConfig.BaseUrl}datastore/initload", "POST");
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    
                    await request.SendWebRequest();
                    
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var response = JsonConvert.DeserializeObject<InitData>(request.downloadHandler.text);
                        return Result<InitData>.Success(response);
                    }
                    
                    return Result<InitData>.Failure(new Error(
                        $"Server error: {request.error}",
                        Error.ErrorType.Server
                    ));
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Get data failed: {ex}");
                return Result<InitData>.Failure(new Error(
                    "Unexpected error occurred",
                    Error.ErrorType.Unknown,
                    ex
                ));
            }
        }
        
        [Serializable] private class EndFishingRequest { public string guid; public bool suc; }
        [Serializable] private class EndFishingResponse { public bool suc; public FishJson fish; }
        [Serializable] private class dataReq { public string userId; }
    }
} 