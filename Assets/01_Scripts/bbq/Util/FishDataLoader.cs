using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FishDataLoader : MonoBehaviour
{
    public static string dataUrl = "https://script.google.com/macros/s/AKfycbz5u3jxyerqkbUFPqY8F7SMa9TR25Huay1iHBf_tcURUKsFcXAr6YnO0gq_OeOF-txI/exec";

    public static void LoadData(System.Action<string> onComplete)
    {
        var go = new GameObject("WebRequestRunner").AddComponent<WebRequestRunner>();
        go.StartCoroutine(go.Download(onComplete));
    }

    private class WebRequestRunner : MonoBehaviour
    {
        public IEnumerator Download(System.Action<string> onComplete)
        {
            UnityWebRequest www = UnityWebRequest.Get(dataUrl);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string wrappedJson = "{\"characters\":" + www.downloadHandler.text + "}";
                print(www.downloadHandler.text);
                // FishDataList list = JsonUtility.FromJson<FishDataList>(wrappedJson);
                onComplete?.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("데이터 다운로드 실패: " + www.error);
            }

            DestroyImmediate(gameObject);
        }
    }
}
