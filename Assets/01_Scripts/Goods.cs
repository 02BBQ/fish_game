using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Goods : MonoBehaviour
{
    Button button;
    public Item item;
    public int cost;
    public Image image;
    public TextMeshProUGUI description;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClickBuy);    
    }

    private void ClickBuy()
    {
        if (Definder.GameManager.Coin >= cost)
        {
            InventoryManager.Instance.AddItem(item);
            Definder.GameManager.Coin -= cost;
        }
    }

    public void SetItem(Item item)
    {
        this.item = item;
        description.text = item.description;
        image.sprite = item.image;
    }
}
