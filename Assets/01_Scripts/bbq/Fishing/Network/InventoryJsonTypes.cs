using System;

[Serializable]
public class ItemJson
{
    public string guid;
    public string name;
    public ItemType type;
    public string visualAddress;
    public string description;
}

[Serializable] public class InventoryData
{
    public FishJson[] Fish;
    // public FishingRod[] rods;
}

[Serializable] public class InitData
{
    public int money;
    public InventoryData inventoryData;
}

// [Serializable]

[Serializable]
public class FishJson: ItemJson
{
    public string spec;
    public string rarity;
    public string trait;
    public float purity;
    public string id;
    public float weight;
    public float price;
}