using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    Image image;
    public Color selectedColor, normalColor;
    [field: SerializeField] public InventoryItem slotItem;

    private void Awake()
    {
        image = GetComponent<Image>();
        Deselect();
    }
    public void Select()
    {
        image.color = selectedColor;
    }
    public void Deselect()
    {
        image.color = normalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // if(transform.childCount == 0)
        // {
        //     InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        //     inventoryItem.parentAfterDrag = transform;
        //     slotItem = inventoryItem;
        // }
        if (slotItem != null)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            slotItem.parentAfterDrag = inventoryItem.parentBeforeDrag;
            slotItem.transform.SetParent(inventoryItem.parentBeforeDrag, false);
            inventoryItem.parentAfterDrag = transform;
            slotItem.SetSlotItem();
            slotItem = inventoryItem;
        }
        else
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
            slotItem = inventoryItem;
        }
    }

    public void ResetItem()
    {
        slotItem = null;
    }
}
