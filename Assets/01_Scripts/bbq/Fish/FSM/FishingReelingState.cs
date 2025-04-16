using System.Text;
using UnityEngine;

public class FishingReelingState : FishingStateBase
{
    private float progress;
    private Vector3 goal;
    private Vector3 p0, p1, p2;

    public FishingReelingState(Fishing fishing) : base(fishing) 
    {
        fishing.Player.playerAnim.SetBool("Fishing", false);
    }

    public override void Enter()
    {
        progress = 0f;
        goal = fishing.transform.position;
        p0 = fishing.FishingVisual.bobber.position;
        p2 = fishing.FishingVisual.fishingRodTip.position;
        p1 = (p0 + p2) / 2; 
        p1.y = p2.y + 2f + Vector3.Distance(p0, p2) / 8;
    }

    public override void Update()
    {
        if (fishing.Success)
        {
            progress = Mathf.Min(progress + Time.deltaTime, 1);
            fishing.FishingVisual.bobber.position = fishing.Bezier(p0, p1, p2, progress);
            
            if (progress >= 1)
            {
                fishing.Player.playerAnim.SetTrigger("BackFlip");
                EndReeling();
            }
        }
        else
        {
            progress = Mathf.Min(progress + Time.deltaTime * 0.75f, 1);
            fishing.FishingVisual.bobber.position = Vector3.Lerp(
                fishing.FishingVisual.bobber.position, goal, progress);
                
            if (progress >= 1)
            {
                EndReeling();
            }
        }
    }

    private void EndReeling()
    {
        if (fishing.Success)
        {
            StringBuilder sb = new StringBuilder(
                $"{fishing.Fish.weight}kg <color=yellow>{fishing.Fish.species}</color>를 낚았다!");
            Events.NotificationEvent.text = sb.ToString();
            EventManager.Broadcast(Events.NotificationEvent);
            InventoryManager.Instance.AddItem(fishing.Fish);
        }
        else
        {
            GameObject.Destroy(fishing.Fish);
        }
        
        fishing.FishingVisual.ResetBobber();
        fishing.PlayerMovement.movable = true;
        fishing.Player.playerSlot.CanChange = true;
        fishing.ChangeState(Fishing.FishingStateType.Idle);
    }
}