public abstract class FishingStateBase
{
    protected Fishing fishing;
    protected FishingStateBase(Fishing fishing) => this.fishing = fishing;
    
    public virtual void Enter() {}
    public virtual void Update() {}
    public virtual void Exit() {}
    public virtual void OnHoldStart() {}
    public virtual void OnHoldEnd() {}
}

public class FishingStateMachine
{
    private FishingStateBase currentState;
    
    public FishingStateBase CurrentState => currentState;
    
    public void Initialize(FishingStateBase startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }
    
    public void ChangeState(FishingStateBase newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
    
    public void Update() => currentState.Update();
    
    public void OnHoldStart() => currentState.OnHoldStart();
    public void OnHoldEnd() => currentState.OnHoldEnd();
}