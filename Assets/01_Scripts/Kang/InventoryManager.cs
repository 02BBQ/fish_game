using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class InventoryManager : SingleTon<InventoryManager>
{
    public int maxStackedItems = 16;
    public GameObject inventoryUI;
    public InventorySlot[] inventorySlots;
    public ItemInfo itemInfo;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private TMP_Dropdown category;
    [SerializeField] private int quickSlotCount = 3;

    [field: SerializeField] public List<Item> Items {get; private set;} = new List<Item>();
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();

    int selectedSlot = -1;

#region UNITY_EVENTS
    private void Awake()
    {
        category.ClearOptions();
        List<string> types = new List<string>();
        types.Add("All");
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            if(type != ItemType.Boat && type != ItemType.None)
                types.Add(type.ToString());
        }
        category.AddOptions(types);
    }

    private void Start()
    {
        category.onValueChanged.AddListener(OnChangeCategory);
        EventBus.Subscribe(EventBusType.Start, Init);
    }

    private void OnDestroy()
    {
        if(Definder.Player)
            Definder.Player.playerInput.downKeyPad -= OnKeypadDown;
        EventBus.Unsubscribe(EventBusType.Start, Init);
    }
#endregion

    private void Init()
    {
        // ChangeSelectedSlot(0);
        Definder.Player.playerInput.downKeyPad += OnKeypadDown;
    }
    private void OnKeypadDown(int key)
    {
        ChangeSelectedSlot(key - 1);
    }

    void ChangeSelectedSlot(int newValue)
    {

        if (!Definder.Player.playerSlot.CanChange || newValue > 3)
        {
            return;
        }
        if (selectedSlot >= 0)
            inventorySlots[selectedSlot].Deselect();

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            InventoryItem itemInSlot = inventoryItems[i];

            if (itemInSlot != null && 
                itemInSlot.item.nameStr == item.nameStr && 
                itemInSlot.count < maxStackedItems &&
                itemInSlot.item.stackable)//��ĥ�� �ִ� �������̸�
            {

                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }
        if (inventoryItems.Count < inventorySlots.Length)//��ĭ ������
        {
            SpawnNewItem(item);
            return true;
        }
        return false;
    }
    public bool RemoveItem(Item item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            InventoryItem itemInSlot = inventoryItems[i];
            if (itemInSlot != null &&
                itemInSlot.item.nameStr == item.nameStr)
            {
                if (item.stackable == true && itemInSlot.count > 1)
                {
                    itemInSlot.count--;
                    itemInSlot.RefreshCount();
                }
                else
                {
                    DeleteItem(itemInSlot);
                }
                return true;
            }
        }
        return false;
    }
    void SpawnNewItem(Item item)
    {
        item = Instantiate(item);
        Items.Add(item);
        GameObject newItemGo = Instantiate(inventoryItemPrefab);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
        inventoryItems.Add(inventoryItem);
        newItemGo.SetActive(false);
        RefreshFillter();
    }

    

    void DeleteItem(InventoryItem slot)
    {
        inventoryItems.Remove(slot);
        Items.Remove(slot.item);
        Destroy(slot.gameObject);
    }

    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.slotItem;

        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;

            if (use)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                    Destroy(itemInSlot.gameObject);
                else
                    itemInSlot.RefreshCount();
            }
            return item;
        }
        return null;
    }

    public void UpdateInfo(Item item)
    {
        if (item == null) return;

        itemInfo.UpdateItemInfo(item);
    }

    #region Fillter
    public ItemType GetCurrentCategory()
    {
        int index = category.value;
        if (index == 0) return ItemType.None;
        string str = category.options[index].text;

        return Enum.Parse<ItemType>(str);
    }
    public void RefreshFillter()
    {
        if (category.value == 0)
            RefreshItemAll();
        else
        {
            int index = category.value;
            string str = category.options[index].text;
            RefreshItembyCategory(Enum.Parse<ItemType>(str));//find itemtype with dropbox value
        }
    }
    void RefreshItembyCategory(ItemType type)
    {
        CleanSlot();

        int slotIndex = quickSlotCount;//그냥 밑에 떠있는 슬롯은 제외하기 위한 것
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            InventoryItem invItem = inventoryItems[i];
            if (invItem.item.type == type && invItem.gameObject.activeSelf == false)
            {
                print("afdasfsdfsdf");
                invItem.gameObject.SetActive(true);
                inventorySlots[slotIndex].SetItem(invItem);
                slotIndex++;
            }
        }
    }

    void RefreshItemAll()
    {
        CleanSlot();

        int slotIndex = quickSlotCount;
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            InventoryItem invItem = inventoryItems[i];
            if (invItem.gameObject.activeSelf == false)
            {
                invItem.gameObject.SetActive(true);
                inventorySlots[slotIndex].SetItem(invItem);
                slotIndex++;
            }
        }
    }
    private void CleanSlot()
    {
        for (int i = quickSlotCount; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot.slotItem == null) continue;
            
            InventoryItem slotItem = slot.slotItem;
            slotItem.gameObject.SetActive(false);
            slot.ResetItem();
        }
    }
    private void OnChangeCategory(int index)
    {
        if (index == 0)
        {
            RefreshItemAll();
            return;
        }
        RefreshItembyCategory((ItemType)index);
    }
    #endregion
}
