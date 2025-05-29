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
        fishing.FishTray.throwPower = 3f;
        fishing.FishTray.trajectoryLine.enabled = true;
    }

    public override void Update()
    {
        fishing.FishTray.throwPower = Mathf.Min(fishing.FishTray.throwPower + Time.deltaTime, 7f);

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
        fishing.FishTray.trajectoryLine.enabled = false;
    }

    public override void Exit()
    {
    }
}