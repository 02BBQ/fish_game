using System.Collections.Generic;
using UnityEngine;

public class FishingRegionManager : MonoBehaviour
{
    public static FishingRegionManager Instance;
    public List<FishingRegion> fishingRegions;

    private void Awake() {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }

    // public FishingRegion GetRegion(string regionName)
    // {
    //     return fishingRegions.Find(region => region.regionName == regionName);
    // }
}
