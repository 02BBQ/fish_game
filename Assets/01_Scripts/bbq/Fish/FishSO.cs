using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Fishing/FishSO")]
public class FishSO : Item
{
    public int price;
    public float weight;
    public string id;
    public string species;

    public void Initialize(FishData fishSO)
    {
        this.id = Guid.NewGuid().ToString();
        this.species = fishSO.spec;
        this.name = fishSO.name;
        this.nameStr = fishSO.name;
        this.weight = fishSO.baseWeight * Random.Range(fishSO.MinWeightMultiplier, fishSO.MaxWeightMultiplier);
        this.price = (int)CalculatePrice(fishSO);
        this.image = fishSO.fishSprite;
    }

    public virtual float CalculatePrice(FishData fishSO)
    {
        return fishSO.basePrice * weight / fishSO.baseWeight;
    }
}