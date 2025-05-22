using System;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Item")]
public class Item : ScriptableObject
{
    public Sprite image;
    public ItemType type;

    public string nameStr;
    public int price;

    [TextArea]
    public string description;
    public bool stackable = true;
    
    [Header("Visual")]
    public string visualPath;
    public virtual StringBuilder GetDescription()
    {
        return new StringBuilder(this.description);
    }

    public virtual string GetName()
    {
        return this.nameStr;
    }
}


public enum ItemType
{
    None = -1,
    Boat = 0,
    Fish = 1,
    FishingRod = 2,
    Bait = 3
}