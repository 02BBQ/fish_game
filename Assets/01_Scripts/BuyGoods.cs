using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class BuyGoods : MonoBehaviour
{
    Button button;
    public Item item;
    public Image image;
    public TextMeshProUGUI description;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClickBuy);    
    }

    private void ClickBuy()
    {
        BuyItem("test", item.name);
        // if (Definder.GameManager.Coin >= item.price)
        // {
        //     if(item.type == ItemType.Boat)
        //     {
        //         if (item.type == ItemType.Boat)
        //             BoatManager.Instance.UnlockBoat(item);
        //         Destroy(gameObject);
        //     }
        //     else
        //         InventoryManager.Instance.AddItem(item);
        //     Definder.GameManager.Coin -= item.price;
        // }
    }

    public void SetItem(Item item)
    {
        this.item = item;
        description.text = item.description;
        costText.text = item.price.ToString() + "<sprite=0>";
        nameText.text = item.name;
        image.sprite = item.image;
    }

    // 서버의 buy API 엔드포인트
    private string buyApiUrl = "http://172.31.2.88:5926/api/store/buy"; // 실제 URL로 변경

    // 아이템 구매 함수
    public void BuyItem(string userId, string itemId)
    {
        StartCoroutine(BuyItemCoroutine(userId, itemId));
    }

    private IEnumerator BuyItemCoroutine(string userId, string itemId)
    {
        // JSON 데이터 생성
        string jsonData = JsonUtility.ToJson(new BuyRequest(userId, itemId));

        // POST 요청 생성
        UnityWebRequest request = new UnityWebRequest(buyApiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 서버 응답 파싱
            BuyResponse response = JsonUtility.FromJson<BuyResponse>(request.downloadHandler.text);
            if (response.success)
            {
                Debug.Log("구매 성공! " + response.message);
                // Load item from Addressables and add to inventory
                StartCoroutine(LoadAndAddItemToInventory(response.item));
            }
            else
            {
                Debug.LogWarning("구매 실패: " + response.message);
                // TODO: 실패 UI 처리
            }
        }
        else
        {
            Debug.LogError("서버 통신 오류: " + request.error);
            // TODO: 네트워크 에러 UI 처리
        }
    }

    private IEnumerator LoadAndAddItemToInventory(StoreItem storeItem)
    {
        // Load the item from Addressables
        var handle = Addressables.LoadAssetAsync<Item>(storeItem.Address);
        yield return handle;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Item loadedItem = handle.Result;
            if (loadedItem != null)
            {
                // Add the item to inventory
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

        // Release the handle
        Addressables.Release(handle);
    }

    // 요청용 클래스
    [System.Serializable]
    public class BuyRequest
    {
        public string userId;
        public string itemId;
        public BuyRequest(string userId, string itemId)
        {
            this.userId = userId;
            this.itemId = itemId;
        }
    }

    // 응답용 클래스 (서버 응답 구조에 맞게 수정)
    [System.Serializable]
    public class BuyResponse
    {
        public bool success;
        public string message;
        public StoreItem item; // 필요하면 상세 구조 추가
    }

    [System.Serializable]
    public struct StoreItem
    {
        public string Address;
        public string Id;
        public string Name;
        public string Category;
        public int CurrencyCount;
        public string Guid;
        public long purchaseDate;  // JavaScript의 Date.now()는 밀리초 단위의 타임스탬프를 반환하므로 long 사용
    }
}
