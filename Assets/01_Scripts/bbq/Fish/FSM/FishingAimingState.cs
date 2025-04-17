using UnityEngine;

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
        fishing.FishTray.trajectoryLine.enabled = true;
    }

    public override void Update()
    {
        if (aiming)
            fishing.FishTray.UpdateTray(rig.transform);
    }

    public override void OnHoldEnd()
    {
        aiming = false;
        fishing.Destination = fishing.FishTray.Goal;
        fishing.Player.playerAnim.SetBool("Fishing", true);
        fishing.PlayerMovement.movable = false;
        fishing.Player.playerSlot.CanChange = false;
        fishing.Success = false;
    }

    public override void Exit()
    {
        fishing.FishTray.trajectoryLine.enabled = false;
    }
}