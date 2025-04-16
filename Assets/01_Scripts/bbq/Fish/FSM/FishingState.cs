using UnityEngine;

public class FishingState : FishingStateBase
{
    private float timeout;

    public FishingState(Fishing fishing) : base(fishing) 
    {
        timeout = Random.Range(5f, 21f);
    }

    public override void Enter()
    {
        base.Enter();
        if (fishing.Hit.collider != null && fishing.Hit.collider.gameObject.layer != LayerMask.NameToLayer("Suimono_Water"))
        {
            fishing.ChangeState(Fishing.FishingStateType.Reeling);
        }
    }

    public override void Update()
    {
        timeout -= Time.deltaTime;
        
        if (timeout <= 0)
        {
            fishing.ChangeState(Fishing.FishingStateType.Fighting);
        }
    }

    public override void OnHoldStart()
    {
        base.OnHoldStart();

        fishing.Success = false;
        fishing.ChangeState(Fishing.FishingStateType.Reeling);
    }
}