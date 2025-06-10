using UnityEngine;
using System.Threading.Tasks;

namespace fishing.FSM
{
    public class FishingState : FishingStateBase
    {
        private float fishingTime = 0f;
        private const float FISHING_DURATION = 3f;
        private bool isWaitingForServer = false;

        public FishingState(Fishing fishing) : base(fishing) { }

        public override async void Enter()
        {
            fishingTime = 0f;
            isWaitingForServer = true;
            
            try
            {
                fishing.FishingFish = fishing.FishingRegion.fishWeights[fishing.GetCurrentRegionIndex() - 1].GetFish();
                
                // 서버에 낚시 시작 요청
                await StartServerFishing(() => 
                {
                    Debug.Log("낚시 준비 완료!");
                    isWaitingForServer = false;
                });
            }
            catch (System.Exception e)
            {
                Debug.LogError($"낚시 시작 중 오류 발생: {e.Message}");
                fishing.ChangeState(Fishing.FishingStateType.Idle);
            }
        }

        public override void Update()
        {
            if (isWaitingForServer) return;

            fishingTime += Time.deltaTime;
            if (fishingTime >= FISHING_DURATION)
            {
                fishing.ChangeState(Fishing.FishingStateType.Fighting);
            }
        }

        public override void Exit()
        {
            fishing.FishingVisual.SetAnchor(false);
            isWaitingForServer = false;
        }
    }
}