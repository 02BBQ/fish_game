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
        // Deselect();
    }

    private void Start()
    {
        Deselect();
    }

    public void Select()
    {   
        var plr = Definder.Player;
        if (plr.playerSlot.currentEquip != null)
        {
            plr.playerSlot.currentEquip.Unequip();
        }
        if (slotItem != null &&slotItem.item is IEquipable equipable)
        {
            equipable.Equip(plr);
        }
        image.color = selectedColor;
    }
    public void Deselect()
    {
        var plr = Definder.Player;
        if (plr.playerSlot.currentEquip != null)
        {
            plr.playerSlot.currentEquip.Unequip();
        }
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
