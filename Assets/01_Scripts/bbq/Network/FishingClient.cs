using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FishingClient : MonoBehaviour
{
    private const string baseUrl = "http://localhost:3000/api/fish";
    
    public IEnumerator StartFishing()
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{baseUrl}/start", ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("StartFishing Error: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                Debug.Log("StartFishing Response: " + json);
                // 필요하면 JsonUtility.FromJson<>()으로 파싱
            }
        }
    }

    public IEnumerator EndFishing(string guid, bool success)
    {
        string jsonData = JsonUtility.ToJson(new EndFishRequest { guid = guid, suc = success });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest($"{baseUrl}/end", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("EndFishing Error: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("EndFishing Response: " + json);
        }
    }

    [System.Serializable]
    private class EndFishRequest
    {
        public string guid;
        public bool suc;
    }
}
