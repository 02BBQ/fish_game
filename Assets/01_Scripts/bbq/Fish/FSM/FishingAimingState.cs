using UnityEngine;

namespace fishing.FSM
{
    public class FishingAimingState : FishingStateBase
    {
        private Transform _aimTransform;
        private Vector3 _origin;
        private Vector3 _direction;
        private GameObject _rig;
        private bool _isAiming = false;

        public FishingAimingState(Fishing fishing) : base(fishing) 
        { 
            _rig = fishing.Player.transform.GetComponentInChildren<Animator>().gameObject;
        }

        public override void Enter()
        {
            _isAiming = true;
            
            fishing.PlayerMovement.StopMoveTarget();
            
            _direction = _rig.transform.forward;
            _origin = fishing.Player.transform.position;
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

            if (_isAiming)
                fishing.FishTray.UpdateTray(_rig.transform);
        }

        public override void OnHoldEnd()
        {
            _isAiming = false;
            fishing.FishTray.trajectoryLine.enabled = false;
            fishing.Destination = fishing.FishTray.Goal;
            fishing.Player.playerAnim.SetBool("Fishing", true);
            fishing.PlayerMovement.movable = false;
            fishing.Player.playerSlot.CanChange = false;
            fishing.Success = false;
            fishing.FishTray.trajectoryLine.enabled = false;
            // fishing.ChangeState(Fishing.FishingStateType.Casting);
        }

        public override void Exit()
        {
        }
    }
}
