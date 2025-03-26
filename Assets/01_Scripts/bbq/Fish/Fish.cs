using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Fishing/FishData")]
public class FishData : ScriptableObject
{
    public Sprite fishSprite;
    [Header("Fish Stat")]
    public float baseWeight;
    // public float size;
    public string spec;
    public float basePrice;
    public GameObject fishPrefab;

    [Header("Info")]
    public float MaxWeightMultiplier;
    public float MinWeightMultiplier;

    public float DancingStepMax;
    public float DancingStepMin;

    [TextArea] public string description;

    public float GetDancingStep()
    {
        return Random.Range(DancingStepMin, DancingStepMax);
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
