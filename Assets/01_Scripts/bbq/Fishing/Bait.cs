using UnityEngine;

[CreateAssetMenu(fileName = "Bait", menuName = "Items/Bait")]
public class Bait : Item
{
    [SerializeField] private int id;
    [SerializeField] private int limit;
    [SerializeField] private int maxPerPurchase;
    [SerializeField] private float baitMultiplier;

    public int Id => id;
    public int Limit => limit;
    public int MaxPerPurchase => maxPerPurchase;
    public float BaitMultiplier => baitMultiplier;
} 