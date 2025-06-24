using UnityEngine;

namespace fishing.FSM
{
    public class FishingCastingState : FishingStateBase
    {
        private float _castingTime = 0f;
        private const float CASTING_DURATION = 1f;
        private float _progress = 0f;
        private Vector3 _p0, _p3;

        public FishingCastingState(Fishing fishing) : base(fishing) { }

        private void UpdateThrow()
        {
            _progress += Time.deltaTime / CASTING_DURATION;
            
            if (_progress >= 1f)
            {
                fishing.FishingVisual.Bobber.position = fishing.Destination;
                fishing.ChangeState(Fishing.FishingStateType.Fishing);
                return; 
            }
            
            // 궤적 포인트 보간
            int index = Mathf.FloorToInt(_progress * (fishing.FishTray.lineResolution - 1));
            float segmentProgress = (_progress * (fishing.FishTray.lineResolution - 1)) - index;
            
            if (index < fishing.FishTray.TrajectoryPoints.Length - 1)
            {
                fishing.FishingVisual.Bobber.position = Vector3.Lerp(fishing.FishTray.TrajectoryPoints[index], fishing.FishTray.TrajectoryPoints[index + 1], segmentProgress);
            }
        }

        public override void Enter()
        {
            _castingTime = 0f;
            _progress = 0f;
            fishing.FishingVisual.ResetBobber();
            fishing.Player.playerAnim.SetBool("Fishing", true);
            fishing.PlayerMovement.movable = false;
            fishing.Player.playerSlot.CanChange = false;
            fishing.UsedBait = fishing.Player.playerSlot.currentBait;
        }

        public override void Update()
        {
            _castingTime += Time.deltaTime;
            if (_castingTime >= CASTING_DURATION)
            {
                fishing.ChangeState(Fishing.FishingStateType.Fishing);
                return;
            }
            UpdateThrow();
        }

        public override void Exit()
        {
            fishing.FishingVisual.SetAnchor(true, fishing.Destination);
        }
    }
}