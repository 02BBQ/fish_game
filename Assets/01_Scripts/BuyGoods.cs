using System;
using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using fishing.Network;
using System.Threading.Tasks;

public class BuyGoods : MonoBehaviour
{
    [Header("Server Config")]
    public ServerConfig serverConfig;

    [Header("UI")]
    Button button;
    public Item item;
    public Image image;
    public TextMeshProUGUI description;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;

    [Serializable] public class BuyRequest
    {
        public string userId;
        public string itemId;
    }

    [Serializable] public class BuyResponse
    {
        public bool success;
        public string message;
        public StoreItem item;
        public uint money;
    }

    [Serializable]
    public struct StoreItem
    {
        public string Address;
        public string Id;
        public string Name;
        public string Category;
        public int CurrencyCount;
        public string Guid;
        public long purchaseDate;
    }

    private IRetryPolicy _retryPolicy;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClickBuy);
        _retryPolicy = new ExponentialBackoffRetryPolicy(serverConfig.MaxRetries, serverConfig.RetryDelaySeconds);
    }

    private async void ClickBuy()
    {
        var result = await BuyItemAsync(serverConfig.DefaultUserId, item.name);

        if (!result.IsSuccess || !result.Data.success)
        {
            // 실패 메시지 만들기
            string reason = result.IsSuccess ? result.Data.message : "서버 연결 실패";
            System.Text.StringBuilder sb = new System.Text.StringBuilder(
                $"<color=yellow>{item.name}</color> 구매 실패: <color=red>{reason}</color>"
            );
            Events.NotificationEvent.text = sb.ToString();
            EventManager.Broadcast(Events.NotificationEvent);
            return;
        }

        System.Text.StringBuilder sucsb = new System.Text.StringBuilder(
                $"<color=yellow>{item.name}</color> 구매 성공!"
            );
            Events.NotificationEvent.text = sucsb.ToString();
            EventManager.Broadcast(Events.NotificationEvent);

        // 성공 시 기존 로직 (아무것도 안 해도 됨)
    }

    public async Task<Result<BuyResponse>> BuyItemAsync(string userId, string itemId)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var requestData = new BuyRequest { userId = userId, itemId = itemId };
            string json = JsonConvert.SerializeObject(requestData);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            using var request = new UnityWebRequest($"{serverConfig.BaseUrl}store/buy", "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            var op = request.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<BuyResponse>(request.downloadHandler.text);
                if (response.success)
                {
                    Definder.GameManager.moneyController.SetMoney((int)response.money);
                    await LoadAndAddItemToInventory(response.item);
                }
                return Result<BuyResponse>.Success(response);
            }
            return Result<BuyResponse>.Failure(new Error($"Server error: {request.error}", Error.ErrorType.Server));
        });
    }

    private async Task LoadAndAddItemToInventory(StoreItem storeItem)
    {
        var handle = Addressables.LoadAssetAsync<Item>(storeItem.Address);
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Item loadedItem = handle.Result;
            if (loadedItem != null)
            {
                InventoryManager.Instance.AddItem(loadedItem);
                Debug.Log($"아이템 '{loadedItem.nameStr}' 인벤토리에 추가됨!");
            }
            else
            {
                Debug.LogError($"아이템 로드 실패: {storeItem.Address}");
            }
        }
        else
        {
            Debug.LogError($"어드레서블 로드 실패: {storeItem.Address}");
        }

        Addressables.Release(handle);
    }

    public void SetItem(Item item)
    {
        this.item = item;
        description.text = item.description;
        costText.text = item.price.ToString() + "<sprite=0>";
        nameText.text = item.name;
        image.sprite = item.image;
    }
}
