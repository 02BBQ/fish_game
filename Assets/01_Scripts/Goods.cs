using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Goods : MonoBehaviour
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
        if (Definder.GameManager.Coin >= item.cost)
        {
            if(item.type == ItemType.Boat)
            {
                Definder.GameManager.UnlockBoat(item.nameStr);
                Destroy(gameObject);
            }
            else
                InventoryManager.Instance.AddItem(item);
            Definder.GameManager.Coin -= item.cost;
        }
    }

    public void SetItem(Item item)
    {
        this.item = item;
        description.text = item.description;
        costText.text = item.cost.ToString() + "<sprite=0>";
        nameText.text = item.name;
        image.sprite = item.image;
    }
}
