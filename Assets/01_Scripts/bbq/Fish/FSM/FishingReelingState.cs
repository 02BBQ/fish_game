using UnityEngine;
using System.Threading.Tasks;
using System.Text;
using System.Collections;

namespace fishing.FSM
{
    public class FishingReelingState : FishingStateBase
    {
        private float reelingTime = 0f;
        private const float REELING_DURATION = 1f;
        private Vector3 goal;
        private Vector3 p0, p1, p2;
        private FishSO fish;
        private bool isProcessingResult = false;

        public FishingReelingState(Fishing fishing) : base(fishing) 
        {
            fishing.Player.playerAnim.SetBool("Fishing", false);
        }

        public override async void Enter()
        {
            reelingTime = 0f;
            isProcessingResult = true;
            
            try
            {
                fishing.Player.playerAnim.SetBool("Fishing", false);
                fishing.PlayerMovement.movable = true;
                fishing.Player.playerSlot.CanChange = true;
                
                SetupReelingAnimation();
                
                // 서버에 결과 전송
                await EndServerFishing(fishing.Success, fishData => 
                {
                    if (fishing.Success && fishData != null)
                    {
                        var fish = GameObject.Instantiate(fishing.FishSOBase);
                        fish.Initialize(fishData);
                        HandleSuccess(fish);
                    }
                    else
                    {
                        HandleFailure();
                    }
                    isProcessingResult = false;
                });
            }
            catch (System.Exception e)
            {
                Debug.LogError($"낚시 결과 처리 중 오류 발생: {e.Message}");
                HandleFailure();
                isProcessingResult = false;
            }
        }

        private void SetupReelingAnimation()
        {
            goal = fishing.transform.position;
            p0 = fishing.FishingVisual.Bobber.position;
            p2 = fishing.FishingVisual.FishingRodTip.position;
            p1 = (p0 + p2) / 2; 
            p1.y = p2.y + 2f + Vector3.Distance(p0, p2) / 8;
        }

        public override void Update()
        {
            if (isProcessingResult) return;

            reelingTime += Time.deltaTime;
            float t = reelingTime / REELING_DURATION;
            
            if (t <= 1f)
            {
                fishing.FishingVisual.Bobber.position = fishing.Bezier(p0, p1, p2, t);
            }
            else
            {
                EndReeling();
            }
        }

        private void HandleSuccess(FishSO fish)
        {
            this.fish = fish;
            try
            {
                if (string.IsNullOrEmpty(fish.trait))
                {
                    StringBuilder sb = new StringBuilder(
                        $"{fish.weight:F1}kg <color=yellow>{fish.species}</color>를 낚았다!");
                    Events.NotificationEvent.text = sb.ToString();
                }
                else
                {
                    StringBuilder sb = new StringBuilder(
                        $"{fish.weight:F1}kg {fish.purity:F1} {fish.trait} <color=yellow>{fish.species}</color>를 낚았다!");
                    Events.NotificationEvent.text = sb.ToString();
                }
                EventManager.Broadcast(Events.NotificationEvent);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"알림 표시 중 오류 발생: {e.Message}");
            }
        }

        private void HandleFailure()
        {
            try
            {
                if (fishing.Fish != null)
                {
                    GameObject.Destroy(fishing.Fish);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"물고기 제거 중 오류 발생: {e.Message}");
            }
        }

        private void EndReeling()
        {
            try
            {
                fishing.FishingVisual.ResetBobber();
                fishing.PlayerMovement.movable = true;
                fishing.Player.playerSlot.CanChange = true;
                fishing.ChangeState(Fishing.FishingStateType.Idle);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"릴링 종료 중 오류 발생: {e.Message}");
            }
        }

        public override void Exit()
        {
            try
            {
                fishing.FishingVisual.ResetBobber();
                isProcessingResult = false;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"상태 종료 중 오류 발생: {e.Message}");
            }
        }
    }
}