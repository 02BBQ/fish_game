using UnityEngine;

namespace fishing.FSM
{
    public class FishingAimingState : FishingStateBase
    {
        private Transform aimTransform;
        private Vector3 origin;
        private Vector3 direction;
        private GameObject rig;
        private bool aiming = false;

        public FishingAimingState(Fishing fishing) : base(fishing) { }

        public override void Enter()
        {
            aiming = true;
            
            fishing.PlayerMovement.StopMoveTarget();
            rig = fishing.Player.transform.GetComponentInChildren<Animator>().gameObject;
            
            direction = rig.transform.forward;
            origin = fishing.Player.transform.position;
            fishing.FishTray.throwPower = 3f;
            fishing.FishTray.trajectoryLine.enabled = true;
        }

        public override void Update()
        {
            if (fishing.IsMouseDown)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100f, fishing.ToAimLayer))
                {
                    fishing.Destination = hit.point;
                    fishing.Distance = Vector3.Distance(fishing.Player.transform.position, hit.point);
                    fishing.Hit = hit;
                }
            }

            fishing.FishTray.throwPower = Mathf.Min(fishing.FishTray.throwPower + Time.deltaTime, 7f);

            if (aiming)
                fishing.FishTray.UpdateTray(rig.transform);
        }

        public override void OnHoldEnd()
        {
            Debug.Log("HoldEnd");
            if (fishing.Distance <= fishing.MaxDistance)
            {
                fishing.ChangeState(Fishing.FishingStateType.Casting);
            }
        }

        public override void Exit()
        {
        }
    }
}