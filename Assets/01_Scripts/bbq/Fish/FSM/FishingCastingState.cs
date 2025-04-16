using UnityEngine;

public class FishingCastingState : FishingStateBase
{
    private float progress;
    private Vector3 p0, p3;

    public FishingCastingState(Fishing fishing) : base(fishing) 
    {
        
    }

    public override void Update()
    {
        progress = Mathf.Min(progress + Time.deltaTime / 
            Mathf.Max((fishing.Destination - p0).magnitude / 10.3f, 0.8f), 1);
        
        var p1 = p0 + Vector3.up * Mathf.Min(Vector3.Distance(p0, p3) / 4f, 3f);
        Vector3 p2 = (p1 + fishing.Destination) / 2; 
        p2.y = p1.y;
        
        var point = fishing.QuadBeizer(p0, p1, p2, fishing.Destination, progress);
        fishing.FishingVisual.bobber.position = point;
        
        if (progress >= 1)
        {
            fishing.ChangeState(Fishing.FishingStateType.Fishing);
        }
    }

    public override void Enter()
    {
        // Debug.Log("asd");
        progress = 0;
        p0 = fishing.transform.position - fishing.transform.forward;
        p3 = fishing.Destination;
        
        // fishing.FishingFish = fishing.FishingRegion.fishWeights[fishing.GetCurrentRegionIndex() - 1].GetFish();
    }

    public override void Exit()
    {
        
    }
}