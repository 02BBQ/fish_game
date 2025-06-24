using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using fishing.FSM;
using bbq.Fishing;

/// <summary>
/// this class is in gray background image, this is slot
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerClickHandler
{
    [field: SerializeField] public Image Image { get; private set; }
    public Color selectedColor, normalColor;
    [field: SerializeField] public InventoryItem slotItem;

    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    private void Start()
    {
        Deselect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var plr = Definder.Player;
        if (!plr.playerSlot.CanChange) return;

        if (slotItem != null && slotItem.item.type == ItemType.Bait)
        {
            // 이미 같은 미끼가 장착되어 있다면 해제
            if (plr.playerSlot.currentBait == slotItem.item)
            {
                BaitEquipSystem.Instance.UnequipBait();
            }
            else
            {
                // 새로운 미끼 장착
                BaitEquipSystem.Instance.EquipBait(slotItem.item);
            }
        }
    }

    public void Select()
    {   
        var plr = Definder.Player;
        if (!plr.playerSlot.CanChange) return;

        if (slotItem != null && slotItem.item is IEquipable equipable)
        {
            if (plr.playerSlot.currentEquip != null)
            {
                plr.playerSlot.currentEquip.Unequip();
            }
            equipable.Equip(plr);
            Image.color = selectedColor;
        }
    }

    public void Deselect()
    {
        var plr = Definder.Player;
        if (!plr.playerSlot.CanChange) return;

        if (slotItem != null)
        {
            if (slotItem.item.type == ItemType.Bait && plr.playerSlot.currentBait == slotItem.item)
            {
                BaitEquipSystem.Instance.UnequipBait();
            }
            else if (plr.playerSlot.currentEquip != null && slotItem.item is IEquipable)
            {
                plr.playerSlot.currentEquip.Unequip();
            }
        }
        Image.color = normalColor;
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
