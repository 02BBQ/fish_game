using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using fishing.Network;

namespace fishing.FSM
{
    public abstract class FishingStateBase
    {
        protected readonly Fishing fishing;
        protected readonly IFishingServerService serverService;

        protected FishingStateBase(Fishing fishing)
        {
            this.fishing = fishing;
            this.serverService = GameManager.Instance.serverService;
        }

        protected async Task StartServerFishing(Action onFishingReady)
        {
            // try 
            // {
                var result = await serverService.StartFishing();
                if (result.IsSuccess)
                {
                    Debug.Log($"낚시 시작: GUID={result.Data.guid}, 시간={result.Data.time}, 댄싱 단계={result.Data.dancingStep}");
                    fishing.UpdateState(result.Data.guid, result.Data.dancingStep);
                    fishing.StartCoroutine(WaitForFishing(result.Data.time / 1000f, onFishingReady));
                }
                else
                {
                    Debug.LogError($"낚시 시작 실패: {result.Error.Message}");
                }
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError($"서버 통신 오류: {e.Message}");
            // }
        }
        
        private IEnumerator WaitForFishing(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);
            callback?.Invoke();
        }

        protected async Task EndServerFishing(bool success, Action<FishJson> onCompleted)
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

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
        public virtual void OnHoldStart() { }
        public virtual void OnHoldEnd() { }
    }
} 