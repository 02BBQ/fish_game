using UnityEngine;

public class FishingAimingState : FishingStateBase
{
    private Transform aimTransform;
    private Vector3 origin;
    private Vector3 direction;
    private GameObject rig;

    public FishingAimingState(Fishing fishing) : base(fishing) { }

    public override void Enter()
    {
        fishing.PlayerMovement.StopMoveTarget();
        rig = fishing.Player.transform.GetComponentInChildren<Animator>().gameObject;
        
        fishing.Aim.SetActive(true);
        direction = rig.transform.forward;
        origin = fishing.Player.transform.position;
        aimTransform = fishing.Aim.transform;
        aimTransform.position = origin;
        fishing.Distance = 1;
    }

    public override void Update()
    {
        origin = fishing.Player.transform.position;
        direction = rig.transform.forward;
        
        fishing.Distance = fishing.IsMouseDown ? 
            Mathf.Min(fishing.Distance + Time.deltaTime, fishing.MaxDistance) : 
            fishing.Distance;
            
        aimTransform.position = origin + direction * fishing.Distance;
        
        if (Physics.Raycast(aimTransform.position + Vector3.up * 4, -Vector3.up, 
            out var hit, 50, fishing.ToAimLayer))
        {
            aimTransform.position = new Vector3(aimTransform.position.x, hit.point.y, aimTransform.position.z);
            fishing.Hit = hit;
        }
    }

    public override void OnHoldEnd()
    {
        fishing.Destination = aimTransform.position;
        fishing.Player.playerAnim.SetBool("Fishing", true);
        fishing.PlayerMovement.movable = false;
        fishing.Player.playerSlot.CanChange = false;
        fishing.Success = false;
    }

    public override void Exit()
    {
        fishing.Aim.SetActive(false);
    }
}