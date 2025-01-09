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
        InventoryManager.Instance.AddItem(item);
    }

    public void SetItem(Item item)
    {
        this.item = item;
        description.text = item.description;
        image.sprite = item.image;
    }
}
