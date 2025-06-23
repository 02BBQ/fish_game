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
using Steamworks;

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

public class GameManager : SingleTon<GameManager>
{
    [Header("Server Config")]
    public ServerConfig serverConfig;

    #region Steam Auth
    [Header("Steam Auth")]
    private bool isSteamInitialized = false;
    private CSteamID steamUserId;
    private HAuthTicket authTicket;
    private byte[] ticketData;

    private void InitializeSteam()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("스팀 초기화 실패! 스팀을 실행하고 다시 시도해주세요.");
            Events.NotificationEvent.text = "<color=red>스팀 초기화 실패! 스팀을 실행하고 다시 시도해주세요.</color>";
            EventManager.Broadcast(Events.NotificationEvent);
            return;
        }

        isSteamInitialized = true;
        steamUserId = SteamUser.GetSteamID();
        Debug.Log($"스팀 초기화 성공! 유저 ID: {steamUserId}");

        // 세션 티켓 발급
        GetSteamAuthTicket();
    }

    private void GetSteamAuthTicket()
    {
        if (!isSteamInitialized)
        {
            Debug.LogError("스팀이 초기화되지 않았습니다!");
            Events.NotificationEvent.text = "<color=red>스팀 연결에 실패했습니다. 스팀을 실행해주세요.</color>";
            EventManager.Broadcast(Events.NotificationEvent);
            return;
        }

        if (steamUserId == CSteamID.Nil)
        {
            Debug.LogError("유효하지 않은 스팀 유저 ID입니다!");
            Events.NotificationEvent.text = "<color=red>스팀에 로그인되어 있지 않습니다.</color>";
            EventManager.Broadcast(Events.NotificationEvent);
            return;
        }

        try
        {
            // 스팀 권장 버퍼 크기 사용 (256바이트면 충분)
            ticketData = new byte[256];
            uint ticketSize;
            var networkingIdentity = new SteamNetworkingIdentity();
            networkingIdentity.SetSteamID(steamUserId);
            
            authTicket = SteamUser.GetAuthSessionTicket(ticketData, ticketData.Length, out ticketSize, ref networkingIdentity);
            
            if (authTicket != HAuthTicket.Invalid && ticketSize > 0)
            {
                string ticketBase64 = Convert.ToBase64String(ticketData, 0, (int)ticketSize);
                SendAuthToServer(steamUserId, ticketBase64);
            }
            else
            {
                Debug.LogError($"스팀 인증 티켓 발급 실패! (티켓: {authTicket}, 크기: {ticketSize})");
                Events.NotificationEvent.text = "<color=red>스팀 인증 실패! 다시 시도해주세요.</color>";
                EventManager.Broadcast(Events.NotificationEvent);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"스팀 인증 티켓 발급 중 오류 발생: {e}");
            Events.NotificationEvent.text = "<color=red>스팀 인증 중 오류가 발생했습니다.</color>";
            EventManager.Broadcast(Events.NotificationEvent);
            
            // 실패한 경우 티켓 정리
            if (authTicket != HAuthTicket.Invalid)
            {
                try
                {
                    SteamUser.CancelAuthTicket(authTicket);
                    authTicket = HAuthTicket.Invalid;
                }
                catch (Exception cancelEx)
                {
                    Debug.LogError($"티켓 취소 중 오류 발생: {cancelEx}");
                }
            }
        }
    }

    private async void SendAuthToServer(CSteamID steamId, string ticketBase64)
    {
        try
        {
            // 서버로 스팀 ID와 인증 티켓 전송
            // 여기서 serverService를 통해 서버에 인증 요청
            var result = await serverService.AuthenticateSteamUser(steamId.ToString(), ticketBase64);
            
            if (result.IsSuccess)
            {
                Debug.Log("스팀 인증 성공!");
                // 인증 성공 후 기존 데이터 로드
                await HandleInitData(result.Data);
            }
            else
            {
                Debug.LogError($"스팀 인증 실패: {result.Error.Message}");
                Events.NotificationEvent.text = $"<color=red>스팀 인증 실패: {result.Error.Message}</color>";
                EventManager.Broadcast(Events.NotificationEvent);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 인증 중 오류 발생: {e.Message}");
            Events.NotificationEvent.text = "<color=red>서버 연결 중 오류가 발생했습니다.</color>";
            EventManager.Broadcast(Events.NotificationEvent);
        }
    }

    private void OnDestroy()
    {
        if (authTicket != HAuthTicket.Invalid)
        {
            SteamUser.CancelAuthTicket(authTicket);
        }
    }
    #endregion

    public Transform spawnPoint;
    public bool startGame = false;
    private AsyncOperationHandle<GameObject> handle;
    private string currentPath;
    private Action<GameObject> currentCallback;


    public MoneyController moneyController;
    public IFishingServerService serverService;

    private void Awake()
    {
        moneyController = GetComponent<MoneyController>();
        serverService = GetComponent<FishingServerService>();
        if (serverService == null)
        { 
            serverService = gameObject.AddComponent<FishingServerService>();
        }
        
        // FishingServerService에 ServerConfig 설정
        var fishingService = serverService as FishingServerService;
        if (fishingService != null)
        {
            fishingService.serverConfig = this.serverConfig;
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
        // Steam 초기화
        InitializeSteam();
    }

    private async Task HandleInitData(InitData data)
    {
        await HandleMoney(data);
        await HandleFishInventory(data.inventoryData);
        await HandleFishingRodInventory(data.inventoryData);
    }

    private Task HandleMoney(InitData data)
    {
        Debug.Log(data.money);
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
            fishingRod = Instantiate(fishingRod);
            
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
