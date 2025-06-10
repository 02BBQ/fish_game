using UnityEngine;

namespace fishing.FSM
{
    public class FishingCastingState : FishingStateBase
    {
        private float castingTime = 0f;
        private const float CASTING_DURATION = 1f;

        public FishingCastingState(Fishing fishing) : base(fishing) { }

        public override void Enter()
        {
            castingTime = 0f;
            fishing.FishingVisual.ResetBobber();
        }

        public override void Update()
        {
            castingTime += Time.deltaTime;
            if (castingTime >= CASTING_DURATION)
            {
                fishing.ChangeState(Fishing.FishingStateType.Fishing);
            }
        }

        public override void Exit()
        {
            fishing.FishingVisual.SetAnchor(true, fishing.Destination);
        }
    }
}