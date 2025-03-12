using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class InventoryManager : SingleTon<InventoryManager>
{
    public int maxStackedItems = 16;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    [field: SerializeField] public List<Item> Items {get; private set;} = new List<Item>();

    int selectedSlot = -1;

    private void Start()
    {
        EventBus.Subscribe(EventBusType.Start, Init);
    }
    private void Init()
    {
        ChangeSelectedSlot(0);
        Definder.Player.playerInput.downKeyPad += OnKeypadDown;
    }

    private void OnDestroy()
    {
        if(Definder.Player)
            Definder.Player.playerInput.downKeyPad -= OnKeypadDown;
        EventBus.Unsubscribe(EventBusType.Start, Init);
    }
    private void OnKeypadDown(int key)
    {
        ChangeSelectedSlot(key - 1);
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
            inventorySlots[selectedSlot].Deselect();

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item)
    {
        if (Items.Count >= inventorySlots.Length) return false;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null && 
                itemInSlot.item == item && 
                itemInSlot.count < maxStackedItems &&
                itemInSlot.item.stackable)
            {

                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
            else if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }
    void SpawnNewItem(Item item, InventorySlot slot)
    {
        item = Instantiate(item);
        Items.Add(item);
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }

    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

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
}
