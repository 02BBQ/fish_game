using UnityEngine;

public class FishingState : FishingStateBase
{
    private float timeout;

    public FishingState(Fishing fishing) : base(fishing) 
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (fishing.FishTray.hit.collider != null && fishing.FishTray.hit.collider.gameObject.layer != LayerMask.NameToLayer("Suimono_Water"))
        {
            fishing.ChangeState(Fishing.FishingStateType.Reeling);
            return;
        }

         StartServerFishing(OnFishingReady);
    }

    private void OnFishingReady()
    {
        if (fishing.CurrentStateType != Fishing.FishingStateType.Fishing) return;
        
        fishing.ChangeState(Fishing.FishingStateType.Fighting);
    }

    public override void Update()
    {
        fishing.fishingVisual.bobber.position = fishing.Destination;
        // fishing.fishingVisual.UpdateTray();
    }

    public override void OnHoldStart()
    {
        base.OnHoldStart();

        fishing.Success = false;
        fishing.ChangeState(Fishing.FishingStateType.Reeling);
    }
}