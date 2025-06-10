using System;
using System.Collections;
using UnityEngine;
using fishing.FSM;
using fishing.Network;

public abstract class FishingStateBase
{
    protected readonly Fishing fishing;
    protected readonly IFishingServerService serverService;

    protected FishingStateBase(Fishing fishing)
    {
        this.fishing = fishing;
        this.serverService = fishing.GetComponent<FishingServerService>();
    }

    protected async void StartServerFishing(Action onFishingReady)
    {
        try
        {
            var result = await serverService.StartFishing();
            if (result.IsSuccess)
            {
                fishing.UpdateState(result.Data.guid, result.Data.dancingStep);
                fishing.StartCoroutine(WaitForFishing(result.Data.time / 1000f, onFishingReady));
            }
            else
            {
                Debug.LogError($"낚시 시작 실패: {result.Error.Message}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 통신 오류: {e.Message}");
        }
    }
    
    private IEnumerator WaitForFishing(float waitTime, Action callback)
    {
        yield return new WaitForSeconds(waitTime);
        callback?.Invoke();
    }
    
    protected async void EndServerFishing(bool success, Action<FishJson> onCompleted)
    {
        try
        {
            if (string.IsNullOrEmpty(fishing.CurrentFishGUID))
            {
                onCompleted?.Invoke(null);
                return;
            }

            var result = await serverService.EndFishing(fishing.CurrentFishGUID, success);
            if (result.IsSuccess)
            {
                onCompleted?.Invoke(result.Data);
            }
            else
            {
                Debug.LogError($"낚시 종료 실패: {result.Error.Message}");
                onCompleted?.Invoke(null);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 통신 오류: {e.Message}");
            onCompleted?.Invoke(null);
        }
    }

    public virtual void Enter() {}
    public virtual void Update() {}
    public virtual void Exit() {}
    public virtual void OnHoldStart() {}
    public virtual void OnHoldEnd() {}
}

namespace fishing.FSM
{
    public class FishingStateMachine
    {
        private FishingStateBase currentState;
        private bool isTransitioning = false;

        public void Initialize(FishingStateBase initialState)
        {
            try
            {
                currentState = initialState;
                currentState?.Enter();
            }
            catch (Exception e)
            {
                Debug.LogError($"상태 초기화 중 오류 발생: {e.Message}");
            }
        }

        public void Update()
        {
            if (isTransitioning || currentState == null) return;

            try
            {
                currentState.Update();
            }
            catch (Exception e)
            {
                Debug.LogError($"상태 업데이트 중 오류 발생: {e.Message}");
            }
        }

        public void ChangeState(FishingStateBase newState)
        {
            if (isTransitioning || newState == null) return;

            try
            {
                isTransitioning = true;
                currentState?.Exit();
                currentState = newState;
                currentState?.Enter();
            }
            catch (Exception e)
            {
                Debug.LogError($"상태 전환 중 오류 발생: {e.Message}");
            }
            finally
            {
                isTransitioning = false;
            }
        }

        public void OnHoldStart()
        {
            if (isTransitioning || currentState == null) return;

            try
            {
                currentState.OnHoldStart();
            }
            catch (Exception e)
            {
                Debug.LogError($"홀드 시작 처리 중 오류 발생: {e.Message}");
            }
        }

        public void OnHoldEnd()
        {
            if (isTransitioning || currentState == null) return;

            try
            {
                currentState.OnHoldEnd();
            }
            catch (Exception e)
            {
                Debug.LogError($"홀드 종료 처리 중 오류 발생: {e.Message}");
            }
        }
    }
}