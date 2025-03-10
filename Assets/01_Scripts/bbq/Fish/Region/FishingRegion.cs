using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishWeight
{
    public FishData fish;
    public int weight;
}

[System.Serializable]
public class WeightTable
{
    public string regionName;
    public List<FishWeight> fishWeights; 

    public FishData GetFish()
    {
        int totalWeight = 0;
        foreach (var fish in fishWeights)
        {
            totalWeight += fish.weight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;
        foreach (var fish in fishWeights)
        {
            currentWeight += fish.weight;
            if (randomWeight < currentWeight)
            {
                return fish.fish;
            }
        }

        return null;
    }
}

[System.Serializable]
[CreateAssetMenu(fileName = "Region", menuName = "SO/Fishing/FishingRegion")]
public class FishingRegion: ScriptableObject
{
    public List<WeightTable> fishWeights;
}

