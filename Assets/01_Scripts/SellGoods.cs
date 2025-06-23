using System;
using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using fishing.Network;
using System.Threading.Tasks;

public class SellGoods : MonoBehaviour
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
    public Item thisItem;

    [Serializable] public class SellItemRequest
    {
        public string userId;
        public string guid;
    }
    [Serializable] public class SellResponse
    {
        public bool success;
        public int money;
    }

    private IRetryPolicy _retryPolicy;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClickSell);
        _retryPolicy = new ExponentialBackoffRetryPolicy(serverConfig.MaxRetries, serverConfig.RetryDelaySeconds);
    }

    private async void ClickSell()
    {
        var result = await SellItemAsync(serverConfig.DefaultUserId, item);

        if (!result.IsSuccess || !result.Data.success)
        {
            // 실패 메시지 만들기
            string reason = result.IsSuccess ? "디버그 귀찮노" : "서버 연결 실패";
            System.Text.StringBuilder sb = new System.Text.StringBuilder(
                $"<color=yellow>{item.name}</color> 구매 실패: <color=red>{reason}</color>"
            );
            Events.NotificationEvent.text = sb.ToString();
            EventManager.Broadcast(Events.NotificationEvent);
            return;
        }

        System.Text.StringBuilder sucsb = new System.Text.StringBuilder(
                $"<color=yellow>{item.name}</color> 판매 성공!"
            );
            Events.NotificationEvent.text = sucsb.ToString();
            EventManager.Broadcast(Events.NotificationEvent);

        Definder.GameManager.moneyController.SetMoney(result.Data.money);
        InventoryManager.Instance.RemoveItem(item);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public async Task<Result<SellResponse>> SellItemAsync(string userId, Item item)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var requestData = new SellItemRequest { userId = userId, guid = item.guid };
            string json = JsonConvert.SerializeObject(requestData);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            using var request = new UnityWebRequest($"{serverConfig.BaseUrl}store/sell", "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            var op = request.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<SellResponse>(request.downloadHandler.text);
                return Result<SellResponse>.Success(response);
            }
            return Result<SellResponse>.Failure(new Error($"Server error: {request.error}", Error.ErrorType.Server));
        });
    }
    
    public void SetItem(Item item)
    {
        this.item = item;
        description.text = item.description;
        costText.text = item.price.ToString() + "<sprite=0>";
        nameText.text = item.name;
        image.sprite = item.image;
        thisItem = item;
    }
}
