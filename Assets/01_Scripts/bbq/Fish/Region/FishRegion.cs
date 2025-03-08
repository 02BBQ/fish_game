using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
    public class FishWeight
    {
        public FishData fish;
        public int weight;
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "Region", menuName = "SO/Fishing/FishingRegion")]
    public class FishingRegion: ScriptableObject
    {
        public string regionName;
        public List<FishWeight> fishWeights;
    }

