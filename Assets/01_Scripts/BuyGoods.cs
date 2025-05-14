using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

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
                // TODO: 인벤토리 갱신, UI 업데이트 등
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
        public ItemData item; // 필요하면 상세 구조 추가
    }

    [System.Serializable]
    public class ItemData
    {
        public string id;
        public string type;
        public int price;
        // 기타 필요한 필드
    }
}
