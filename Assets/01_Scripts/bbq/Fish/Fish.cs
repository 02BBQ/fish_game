using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Fishing/FishData")]
public class FishData : ScriptableObject
{
    [Header("Fish Stat")]
    public float baseWeight;
    // public float size;
    public string spec;
    public float basePrice;
    public string id;
    public GameObject fishPrefab;

    [Header("Info")]
    public float MaxWeightMultiplier;
    public float MinWeightMultiplier;
}

[CreateAssetMenu(fileName = "Item", menuName = "SO/Fishing/FishSO")]
public class FishSO : Item
{
    public int price;
    public float weight;
    public string id;
    public string species;

    public void Initialize(FishData fishSO)
    {
        this.species = fishSO.spec;
        this.weight = fishSO.baseWeight * Random.Range(fishSO.MinWeightMultiplier, fishSO.MaxWeightMultiplier);
        this.price = (int)CalculatePrice(fishSO);
    }

    public virtual float CalculatePrice(FishData fishSO)
    {
        return fishSO.basePrice * weight / fishSO.baseWeight;
    }
}

// public class Fish
// {
//     public int price;
//     public float weight;
//     public string id;
//     public string species;

//     public Fish(FishSO fishSO)
//     {
//         this.species = fishSO.nameStr;
//         this.weight = fishSO.baseWeight * Random.Range(fishSO.MinWeightMultiplier, fishSO.MaxWeightMultiplier);
//         this.price = (int)CalculatePrice(fishSO);
//     }

//     public virtual float CalculatePrice(FishSO fishSO)
//     {
//         return fishSO.basePrice * weight / fishSO.baseWeight;
//     }
// }
