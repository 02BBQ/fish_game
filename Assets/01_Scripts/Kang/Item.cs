using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Item")]
public class Item : ScriptableObject
{
    public Sprite image;
    public ItemType type;

    [TextArea]
    public string description;

    public bool stackable = true;
}


public enum ItemType
{
    Fishing,
    Bait,
    Fish
}