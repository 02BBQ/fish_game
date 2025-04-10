[System.Serializable]
public struct FishStruct
{
    public int id;
    public int key;
    public string spec;
    public string koreanName;
    public float baseWeight;
    public float basePrice;
    public float maxWeightMultiplier;
    public float minWeightMultiplier;
    public float dancingStepMax;
    public float dancingStepMin;
    public string description;
    public string rarity;
}

[System.Serializable]
public class FishStructTable
{
    public FishStruct[] items;
}
