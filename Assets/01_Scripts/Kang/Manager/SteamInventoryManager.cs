using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SteamInventoryManager : SingleTon<SteamInventoryManager>
{
    private SteamInventoryResult_t _inventoryResult = SteamInventoryResult_t.Invalid;
    private Dictionary<SteamItemDef_t, SteamItemDetails_t> _inventoryItems = new Dictionary<SteamItemDef_t, SteamItemDetails_t>();

    [Header("Settings")]
    public bool autoInitialize = true;

    private void Start()
    {
        if (autoInitialize)
        {
            //InitializeSteamInventory();
        }
    }

    public void InitializeSteamInventory()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam not initialized!");
            return;
        }

        SteamInventory.LoadItemDefinitions();
        RefreshInventory();
    }
    public void RefreshInventory()
    {
        if (SteamInventory.GetAllItems(out _inventoryResult))
        {
            uint itemCount = 0;
            SteamInventory.GetResultItems(_inventoryResult, null, ref itemCount);
            if (itemCount > 0)
            {
                SteamItemDetails_t[] items = new SteamItemDetails_t[itemCount];
                SteamInventory.GetResultItems(_inventoryResult, items, ref itemCount);
                _inventoryItems.Clear();
                foreach (var item in items)
                {
                    _inventoryItems[item.m_iDefinition] = item;
                }

                Debug.Log($"Inventory refreshed. Found {itemCount} items.");
            }
        }
    }

    public bool GetItemDefinitionProperty(SteamItemDef_t itemDef, string name, out string price, ref uint buffSize)
    {
        name = "";
        price = "";
        return SteamInventory.GetItemDefinitionProperty(itemDef, name, out price, ref buffSize);
    }

    public bool HasItem(SteamItemDef_t itemDef)
    {
        return _inventoryItems.ContainsKey(itemDef);
    }

    public bool TryGetItemDetails(SteamItemDef_t itemDef, out SteamItemDetails_t details)
    {
        return _inventoryItems.TryGetValue(itemDef, out details);
    }

    public void AddItem(SteamItemDef_t itemDef, uint quantity = 1, Action<bool> callback = null)
    {
        SteamItemDef_t[] itemDefs = new SteamItemDef_t[] { itemDef };
        uint[] quantities = new uint[] { quantity };

        if (SteamInventory.AddPromoItem(out SteamInventoryResult_t result, itemDef))
        {
            SteamInventory.DestroyResult(_inventoryResult);
            _inventoryResult = result;
            callback?.Invoke(true);
            Debug.Log($"Added item {itemDef} x{quantity} to inventory");
        }
        else
        {
            callback?.Invoke(false);
            Debug.LogError("Failed to add item to inventory");
        }
        RefreshInventory();
    }

    public void ConsumeItem(SteamItemInstanceID_t itemId, uint quantity = 1, Action<bool> callback = null)
    {
        SteamInventory.ConsumeItem(out SteamInventoryResult_t result, itemId, quantity);
        SteamInventory.DestroyResult(_inventoryResult);
        _inventoryResult = result;
        callback?.Invoke(true);
        RefreshInventory();
    }

    private void OnDestroy()
    {
        if (_inventoryResult != SteamInventoryResult_t.Invalid)
        {
            SteamInventory.DestroyResult(_inventoryResult);
        }
    }
}