using System;
using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SellGoods : MonoBehaviour
{
    Button button;
    public Item item;
    public Image image;
    public TextMeshProUGUI description;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;
    public Item thisItem;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClickSell);    
    }

    private void ClickSell()
    {
        if (item.type != ItemType.Boat)
        {
            // if(InventoryManager.Instance.RemoveItem(item))
            //     Definder.GameManager.moneyController.EarnMoney(item.price);
            
        }
    }

    [Serializable] public class sellItem { public string userId; public string guid; }
    [Serializable] public class sellResponse { public bool suc; public int money; }

    public void SellItem(string userId, Item itme)
    {
        StartCoroutine(SellItemCoroutine(userId, itme));
    }


    private IEnumerator SellItemCoroutine(string userId, Item item)
    {
            var requestData = new sellItem { userId = userId, guid =  item.guid};
        string json = JsonConvert.SerializeObject(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest($"http://172.31.2.88:5926/api/store/sell", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // brr brr
            if (request.result == UnityWebRequest.Result.Success)
            {
                // fishesJson 클래스를 사용하여 파싱
                var response = JsonConvert.DeserializeObject<InitData>(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
            }
        }
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
