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
    public float purity;
    public string guid;
    public string trait;

    public void Initialize(FishData data)
    {
        // this.id = Guid.NewGuid().ToString();
        this.species = data.spec;
        this.name = data.name;
        this.nameStr = data.name;
        this.weight = data.baseWeight * Random.Range(data.MinWeightMultiplier, data.MaxWeightMultiplier);
        this.price = (int)CalculatePrice(data);
        this.image = data.fishSprite;
        this.description = data.description;
        this.visualPath = data.visualAddress;
    }

    public void Initialize(FishJson data)
    {
        // this.id = Guid.NewGuid().ToString();
        this.id = data.id;
        this.species = data.spec;
        this.name = data.spec;
        this.nameStr = data.name;
        this.weight = data.weight;
        this.price = (int)data.price;
        this.description = data.description;
        this.visualPath = data.visualAddress;
        this.rarity = data.rarity;
        this.trait = data.trait;
        this.purity = data.purity;
        this.type = data.type;
        this.guid = data.guid;
    }

    public override StringBuilder GetDescription()
    {
        if (this.trait != null && this.trait != string.Empty)
        {
            return new StringBuilder("Weight: " + this.weight + "kg" + "\nWorth: " + this.price + "\n"  + "\nTrait: " + this.trait + "\nPurity: " + this.purity + "\n" + this.description);
        }
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