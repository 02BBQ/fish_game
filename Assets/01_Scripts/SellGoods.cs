using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
            if(InventoryManager.Instance.RemoveItem(item))
                Definder.GameManager.Coin += item.price;
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
