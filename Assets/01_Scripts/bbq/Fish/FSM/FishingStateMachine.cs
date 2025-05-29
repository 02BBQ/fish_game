using System;
using System.Collections;
using UnityEngine;

public abstract class FishingStateBase
{
    protected Fishing fishing;
    protected FishingStateBase(Fishing fishing) => this.fishing = fishing;

    protected void StartServerFishing(Action onFishingReady)
    {
        FishingServerConnector.Instance.StartFishing(
            (guid, waitTime, step) => 
            {
                fishing.currentFishGuid = guid;
                fishing.StartCoroutine(WaitForFishing(waitTime, onFishingReady));
                fishing.dancingStep = step;
            },
            error => Debug.LogError($"Fishing start failed: {error}"));
    }
    
    private IEnumerator WaitForFishing(float waitTime, Action callback)
    {
        yield return new WaitForSeconds(waitTime);
        callback?.Invoke();
    }
    
    protected void EndServerFishing(bool success, Action<FishJson> onCompleted)
    {
        if (string.IsNullOrEmpty(fishing.currentFishGuid))
        {
            onCompleted?.Invoke(null);
            return;
        }
        
        FishingServerConnector.Instance.EndFishing(
            fishing.currentFishGuid, 
            success,
            fishData => onCompleted?.Invoke(fishData),
            error => Debug.LogError($"Fishing end failed: {error}"));
    }

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