[System.Serializable]
public struct FishStruct
{
    public int id;
    public string key;
    public string spec;
    public string koreanName;
    public int baseWeight;
    public int basePrice;
    public int maxWeightMultiplier;
    public int minWeightMultiplier;
    public float dancingStepMax;
    public float dancingStepMin;
    public string description;
    public string rarity;
    public string imgUri;
    public string visualAddress;
}

[System.Serializable]
public struct RarityWeight
{
    public string rarity;
    public float weight;
}

[System.Serializable]
public class FishStructTable
{
    public FishStruct[] items;
}

[System.Serializable]
public class RarityWeightTable
{
    public RarityWeight[] items;
}