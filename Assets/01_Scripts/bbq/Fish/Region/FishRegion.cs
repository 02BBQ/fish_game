using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
    public class FishWeight
    {
        public Fish fish;
        public int weight;
    }

    [System.Serializable]
    public class FishingRegion
    {
        public string regionName;
        public List<FishWeight> fishWeights;
    }

