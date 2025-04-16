public class FishingIdleState : FishingStateBase
{
    public FishingIdleState(Fishing fishing) : base(fishing) { }

    public override void OnHoldStart()
    {
        fishing.ChangeState(Fishing.FishingStateType.Aiming);
    }
}