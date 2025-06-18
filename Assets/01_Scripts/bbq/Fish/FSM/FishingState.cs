using UnityEngine;
using System.Threading.Tasks;
using fishing.Network;

namespace fishing.FSM
{
    public class FishingState : FishingStateBase
    {
        private float _fishingTime = 0f;
        private const float FISHING_DURATION = 3f;
        private bool _isWaitingForServer = false;

        public FishingState(Fishing fishing) : base(fishing) { }

        public override async void Enter()
        {
            _fishingTime = 0f;
            _isWaitingForServer = true;
            fishing.Player.playerAnim.SetBool("Fishing", true);
            fishing.PlayerMovement.movable = false;
            fishing.Player.playerSlot.CanChange = false;
            
            // try
            // {
                await StartServerFishing(() => 
                {
                    Debug.Log("낚시 준비 완료!");
                    _isWaitingForServer = false;
                }); 
            // }
            // catch (System.Exception e)
            // {
            //     Debug.LogError($"낚시 시작 중 오류 발생: {e.Message}");
            //     fishing.ChangeState(Fishing.FishingStateType.Idle);
            // }
        }

        public override void Update()
        {
            fishing.FishingVisual.Bobber.position = fishing.Destination;
            
            if (_isWaitingForServer) return;

            _fishingTime += Time.deltaTime;
            if (_fishingTime >= FISHING_DURATION)
            {
                fishing.ChangeState(Fishing.FishingStateType.Fighting);
            }
        }

        public override void Exit()
        {
            fishing.FishingVisual.SetAnchor(false);
            _isWaitingForServer = false;
            fishing.Player.playerAnim.SetBool("Fishing", false);
            fishing.PlayerMovement.movable = true;
            fishing.Player.playerSlot.CanChange = true;
        }

        public override void OnHoldStart()
        {
            base.OnHoldStart();
            fishing.Success = false;
            fishing.ChangeState(Fishing.FishingStateType.Reeling);
        }
    }
}