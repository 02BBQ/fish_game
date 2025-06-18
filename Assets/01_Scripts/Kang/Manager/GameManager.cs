using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ServerData;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using fishing.Network;
using System.Threading.Tasks;

namespace ServerData
{
    [System.Serializable]
    public class InventoryItemData
    {
        public string guid;
        public string Address;
        public string Category;
        public int? CurrencyCount;
        public string Desc;
        public int Id;
        public int Limit;
        public int MaxPerPurchase;
        public string Name;
        public bool Stackable;
        // public ServerItem item;
    }

    [System.Serializable]
    public class ServerResponse
    {
        public int money;
        public InventoryData inventoryData;
    }
}
public class GameManager : MonoBehaviour
{
    [Header("Server Config")]
    public ServerConfig serverConfig;

    public static GameManager Instance { get; private set; }
    public Transform spawnPoint;
    public bool startGame = false;
    private AsyncOperationHandle<GameObject> handle;
    private string currentPath;
    private Action<GameObject> currentCallback;


    public MoneyController moneyController;
    public IFishingServerService serverService;

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

        moneyController = GetComponent<MoneyController>();
        serverService = GetComponent<FishingServerService>();
        if (serverService == null)
        {
            serverService = gameObject.AddComponent<FishingServerService>();
        }
    }

    private async void Start()
    {
        var result = await serverService.GetData(serverConfig.DefaultUserId);
        if (result.IsSuccess)
        {
            await HandleInitData(result.Data);
        }
        else
        {
            Debug.LogError($"Failed to get data: {result.Error.Message}");
        }
        
        Time.timeScale = 1f;
        Debug.Log("GameManager Start");
    }

    private async Task HandleInitData(InitData data)
    {
        await HandleMoney(data);
        await HandleFishInventory(data.inventoryData);
        await HandleFishingRodInventory(data.inventoryData);
    }

    private Task HandleMoney(InitData data)
    {
        moneyController?.SetMoney(data.money);
        return Task.CompletedTask;
    }

    private Task HandleFishInventory(InventoryData inventoryData)
    {
        if (inventoryData.Fish == null) return Task.CompletedTask;

        foreach (FishJson fish in inventoryData.Fish)
        {
            FishSO so = ScriptableObject.CreateInstance<FishSO>();
            so.Initialize(fish);
            InventoryManager.Instance.AddItem(so);
        }
        return Task.CompletedTask;
    }

    private async Task HandleFishingRodInventory(InventoryData inventoryData)
    {
        if (inventoryData.FishingRod == null) return;

        var loadTasks = new List<Task>();
        foreach (InventoryItemData rod in inventoryData.FishingRod)
        {
            loadTasks.Add(LoadFishingRod(rod));
        }
        await Task.WhenAll(loadTasks);
    }

    private async Task LoadFishingRod(InventoryItemData rodData)
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<FishingRod>(rodData.Address);
            var fishingRod = await handle.Task;
            
            if (fishingRod != null)
            {
                fishingRod.guid = rodData.guid;
                InventoryManager.Instance.AddItem(fishingRod);
            }
            else
            {
                Debug.LogError($"FishingRod component not found on prefab: {rodData.Address}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load fishing rod from address {rodData.Address}: {ex.Message}");
        }
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
        EditorApplication.ExitPlaymode();  // �����Ϳ��� ���� ���̸� Play ��� ����
#else
        Application.Quit();  // ����� ���ӿ����� ���� ����
#endif
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
            print("����;;");
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
            Debug.LogError($"�ε忡 �����߽��ϴ�.");
        }
    }

}
