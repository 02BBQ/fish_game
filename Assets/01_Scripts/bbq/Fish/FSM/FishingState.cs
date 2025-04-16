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
        if (fishing.Hit.collider != null && fishing.Hit.collider.gameObject.layer != LayerMask.NameToLayer("Suimono_Water"))
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
    }

    public override void OnHoldStart()
    {
        base.OnHoldStart();

        fishing.Success = false;
        fishing.ChangeState(Fishing.FishingStateType.Reeling);
    }
}