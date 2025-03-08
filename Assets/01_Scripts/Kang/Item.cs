using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Item")]
public class Item : ScriptableObject
{
    public Sprite image;
    public ItemType type;

    public string nameStr;
    public int cost;

    [TextArea]
    public string description;

    public bool stackable = true;
}


public enum ItemType
{
    Boat = 0,
    Fish = 1,
    Fishing = 2,
    Bait = 3
}