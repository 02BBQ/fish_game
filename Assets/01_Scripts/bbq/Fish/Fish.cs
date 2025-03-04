using UnityEngine;

public class Fish
{
    public string species;
    public float weight;
    public float size;
    public float basePrice;
    public string key;

    public Fish(string species, float weight, float size, float basePrice)
    {
        this.species = species;
        this.weight = weight;
        this.size = size;
        this.basePrice = basePrice;
    }

    public float CalculatePrice()
    {
        return 0.0f;
    }
}
