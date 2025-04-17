using UnityEngine;

public class FishingCastingState : FishingStateBase
{
    private float progress;
    private Vector3 p0, p3;
    private float totalFlightTime = 1f;

    public FishingCastingState(Fishing fishing) : base(fishing) 
    {
        
    }

    void StartThrow()
    {
        progress = 0f;
    }

    void UpdateThrow()
    {
        progress += Time.deltaTime / totalFlightTime;
        
        if (progress >= 1f)
        {
            fishing.FishingVisual.bobber.position = fishing.Destination;
            fishing.ChangeState(Fishing.FishingStateType.Fishing);
            return;
        }
        
        // 궤적 포인트 보간
        int index = Mathf.FloorToInt(progress * (fishing.FishTray.lineResolution - 1));
        float segmentProgress = (progress * (fishing.FishTray.lineResolution - 1)) - index;
        
        if (index < fishing.FishTray.TrajectoryPoints.Length - 1)
        {
            fishing.FishingVisual.bobber.position = Vector3.Lerp(fishing.FishTray.TrajectoryPoints[index], fishing.FishTray.TrajectoryPoints[index + 1], segmentProgress);
        }
    }

    public override void Update()
    {
        UpdateThrow();
        // progress = Mathf.Min(progress + Time.deltaTime / 
        //     Mathf.Max((fishing.Destination - p0).magnitude / 10.3f, 0.8f), 1);
        
        // var p1 = p0 + Vector3.up * Mathf.Min(Vector3.Distance(p0, p3) / 4f, 3f);
        // Vector3 p2 = (p1 + fishing.Destination) / 2; 
        // p2.y = p1.y;
        
        // var point = fishing.QuadBeizer(p0, p1, p2, fishing.Destination, progress);
        // fishing.FishingVisual.bobber.position = point;
        
        // if (progress >= 1)
        // {
        //     fishing.ChangeState(Fishing.FishingStateType.Fishing);
        // }
    }

    public override void Enter()
    {
        StartThrow();

        progress = 0;
        p0 = fishing.transform.position - fishing.transform.forward;
        p3 = fishing.Destination;
        
        // fishing.FishingFish = fishing.FishingRegion.fishWeights[fishing.GetCurrentRegionIndex() - 1].GetFish();
    }

    public override void Exit()
    {
        
    }
}