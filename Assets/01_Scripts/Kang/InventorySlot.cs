using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// this class is in gray background image, this is slot
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler
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
        if (!plr.playerSlot.CanChange) return;
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
        if (!plr.playerSlot.CanChange) return;
        if (plr.playerSlot.currentEquip != null)
        {
            plr.playerSlot.currentEquip.Unequip();
        }
        image.color = normalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (slotItem != null)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if (inventoryItem == null) return;

            slotItem.parentAfterDrag = inventoryItem.parentBeforeDrag;
            slotItem.transform.SetParent(inventoryItem.parentBeforeDrag, false);
            inventoryItem.parentAfterDrag = transform;
            slotItem.SetSlotItem();
            slotItem = inventoryItem;
        }
        else
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if (inventoryItem == null) return;
            inventoryItem.parentAfterDrag = transform;
            slotItem = inventoryItem;
        }
    }

    public void ResetItem()
    {
        slotItem = null;
    }
    public void SetItem(InventoryItem item)
    {
        slotItem = item;
        item.SetParent(transform);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotItem != null && slotItem.item != null)
        {
            InventoryManager.Instance.UpdateInfo(slotItem.item);
        }
    }
}
