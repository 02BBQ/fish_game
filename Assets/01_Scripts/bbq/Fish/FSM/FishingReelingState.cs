using System.Text;
using UnityEngine;

public class FishingReelingState : FishingStateBase
{
    private float progress;
    private Vector3 goal;
    private Vector3 p0, p1, p2;
    private FishSO fish;

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
        
        // 서버에 결과 전송
        EndServerFishing(fishing.Success, fishData => 
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
        });
    }

    private void HandleSuccess(FishSO fish)
    {
        this.fish = fish;
        // StringBuilder sb = new StringBuilder($"{fish.weight}kg <color=yellow>{fish.species}</color>를 낚았다!");
        // Events.NotificationEvent.text = sb.ToString();
        // EventManager.Broadcast(Events.NotificationEvent);
        InventoryManager.Instance.AddItem(fish);
        
        // fishing.ChangeState(Fishing.FishingStateType.Idle);
    }
    
    private void HandleFailure()
    {
        // fishing.ChangeState(Fishing.FishingStateType.Idle);
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
        if (fishing.Success && fish != null)
        {
            if (fish.trait == null || fish.trait == string.Empty)
            {
                StringBuilder sb = new StringBuilder(
                    $"{fish.weight}kg <color=yellow>{fish.species}</color>를 낚았다!");
                Events.NotificationEvent.text = sb.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder(
                    $"{fish.weight}kg {fish.purity} {fish.trait} <color=yellow>{fish.species}</color>를 낚았다!");
                Events.NotificationEvent.text = sb.ToString();
            }
            // InventoryManager.Instance.AddItem(fishing.Fish);
            EventManager.Broadcast(Events.NotificationEvent);
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