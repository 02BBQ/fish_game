
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "SO/Fishing/RarityTable")]
public class RarityTable : ScriptableObject
{
    public RarityData[] rarities;

    public FishData GetFish()
    {
        // Calculate total weight
        float totalWeight = 0f;
        foreach (var r in rarities)
        {
            totalWeight += r.weight;
        }

        // Generate a random value between 0 and totalWeight
        float randomValue = Random.Range(0, totalWeight);

        // Find the rarity based on the random value
        float cumulativeWeight = 0f;
        foreach (var r in rarities)
        {
            cumulativeWeight += r.weight;
            if (randomValue <= cumulativeWeight)
            {
                // Return a random fish from the selected rarity
                return r.fishes[Random.Range(0, r.fishes.Length)];
            }
        }

        return null; // Fallback in case no rarity is found
    }
}


[System.Serializable]
public struct RarityData
{
    public string name;
    public float weight;
    public FishData[] fishes;
}