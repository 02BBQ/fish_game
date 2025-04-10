using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Text;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Fishing/FishSO")]
public class FishSO : Item, ModelView
{
    public int price;
    public float weight;
    public string id;
    public string species;

    public string rarity;

    public void Initialize(FishData data)
    {
        this.id = Guid.NewGuid().ToString();
        this.species = data.spec;
        this.name = data.name;
        this.nameStr = data.name;
        this.weight = data.baseWeight * Random.Range(data.MinWeightMultiplier, data.MaxWeightMultiplier);
        this.price = (int)CalculatePrice(data);
        this.image = data.fishSprite;
        this.description = data.description;
        this.visualPath = data.visualAddress;
    }

    public override StringBuilder GetDescription()
    {
        return new StringBuilder("Weight: " + this.weight + "kg" + "\nWorth: " + this.price + "\n"  + this.description);
    }

    public override string GetName()
    {
        return species;
    }

    public virtual float CalculatePrice(FishData fishSO)
    {
        return fishSO.basePrice * weight / fishSO.baseWeight;
    }
}