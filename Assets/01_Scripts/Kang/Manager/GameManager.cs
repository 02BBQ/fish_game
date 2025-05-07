using System;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class GameManager : MonoBehaviour
{
    int _coin;
    public int Coin { 
        get => _coin;
        set
        {
            _coin = value;
            SetCoinText();
        }
    }

    [SerializeField] private TextMeshProUGUI coinText;
    public Transform spawnPoint;
    public bool startGame = false;
    private AsyncOperationHandle<GameObject> handle;
    private string currentPath;
    private Action<GameObject> currentCallback;

    private void Start()
    {
        Time.timeScale = 1f;
        Coin = 100000;
        SetCoinText();
    }
    [ContextMenu("dsfa")]
    public void Delete()
    {
        PlayerPrefs.DeleteAll();
    }
    public void OnClickStart()
    {
        EventBus.Publish(EventBusType.Start);
        Definder.Player.playerMovement.movable = true;
        startGame = true;
        UIManager.Instance.PlayUIIn();
        UIManager.Instance.MainUIOut();
    }
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();  // 에디터에서 실행 중이면 Play 모드 종료
#else
        Application.Quit();  // 빌드된 게임에서는 정상 종료
#endif
    }
    private void SetCoinText()
    {
        coinText.text = _coin.ToString("0");
    }

    public void PauseGame()
    {
        if (!startGame) return; 

        Time.timeScale = 0f;
        UIManager.Instance.PauseUIIn();
        SoundManager.Instance.Pause();
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        UIManager.Instance.PauseUIOut();
        SoundManager.Instance.Unpause();
    }
    public void ReloadScene()
    {
        SceneManager.LoadSceneAsync(gameObject.scene.name);
    }

    public void LoadAddressableAsset(string path, Action<GameObject> callback)
    {
        currentPath = path;
        currentCallback = callback;

        Addressables.LoadResourceLocationsAsync(path).Completed += OnResourceLocationLoaded;
    }
    private void OnResourceLocationLoaded(AsyncOperationHandle<IList<IResourceLocation>> locationHandle)
    {
        if (locationHandle.Status == AsyncOperationStatus.Succeeded && locationHandle.Result.Count > 0)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            handle = Addressables.LoadAssetAsync<GameObject>(currentPath);
            handle.Completed += OnModelLoaded;

        }
        else
        {
            print("없음;;");
        }
        
        Addressables.Release(locationHandle);
    }
    private void OnModelLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            currentCallback?.Invoke(obj.Result);
        }
        else
        {
            Debug.LogError($"로드에 실패했습니다.");
        }
    }

}
