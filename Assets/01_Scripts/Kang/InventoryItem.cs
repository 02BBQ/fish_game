using System;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// this class is in inventory item image, this is Instantiate item image
/// </summary>
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    Image image;
    RectTransform rectTrm;
    public TextMeshProUGUI countText;

     public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Transform parentBeforeDrag;

    public void InitializeItem(Item newItem)
    {
        if (image == null)
        {
            image = GetComponent<Image>();
            rectTrm = GetComponent<RectTransform>();
        }
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }
    public void RefreshCount()
    {
        countText.text = count.ToString("0");
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentBeforeDrag = transform.parent;
        var slot = parentBeforeDrag.GetComponent<InventorySlot>();
        if (slot)
        {
            slot.ResetItem();
        }
        parentAfterDrag = transform.parent;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        ItemType currentType = InventoryManager.Instance.GetCurrentCategory();
        if (item.type == currentType || currentType == ItemType.None)
        {
            parentAfterDrag.GetComponent<InventorySlot>().SetItem(this);
        }
        else
        {
            gameObject.SetActive(false);
            InventoryManager.Instance.RefreshFillter();
        }
    }

    public void SetParent(Transform slot)
    {
        transform.SetParent(slot);
        rectTrm.anchoredPosition = Vector2.zero;
    }

    public void SetSlotItem()
    {
        var slot = transform.parent.GetComponent<InventorySlot>();
        if (slot == null) return;
        slot.slotItem = this;
    }

    
}
