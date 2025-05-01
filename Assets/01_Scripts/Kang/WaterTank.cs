using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterTank : Interactor
{
    public List<FishSO> fishs;
    public FishModel fishPref;
    List<FishModel> pool;

    private void Awake()
    {
       // fishs = new List<FishSO>();
        pool = new List<FishModel>();
        //@ get fishlist in server
    }
    private void OnEnable()
    {
        InventoryManager.Instance.OnAddFish += AddFish;
        InventoryManager.Instance.OnRemoveFish += RemoveFish;
    }
    private void OnDisable()
    {
        InventoryManager.Instance.OnAddFish -= AddFish;
        InventoryManager.Instance.OnRemoveFish -= RemoveFish;
    }
    protected override void Start()
    {
        base.Start();
        // 초기 물고기들 처리
        foreach (FishSO fish in fishs)
        {
            FishModel fishObj = pool.FirstOrDefault(obj => !obj.gameObject.activeSelf);

            if (fishObj == null)
            {
                // 새 오브젝트 생성
                fishObj = Instantiate(fishPref, transform);
                pool.Add(fishObj);
            }
            fishObj.Init(fish.image, fish);
            fishObj.gameObject.SetActive(true);
        }
    }

    public void AddFish(List<FishSO> lst, FishSO fish)
    {
        if (lst != fishs) return;

        fishs.Add(fish);
        
        // pool에서 비활성화된 오브젝트 찾기
        FishModel fishObj = pool.FirstOrDefault(obj => !obj.gameObject.activeSelf);
        
        if (fishObj == null)
        {
            // 새 오브젝트 생성
            fishObj = Instantiate(fishPref, transform);
            pool.Add(fishObj);
        }
        fishObj.Init(fish.image, fish);
        fishObj.gameObject.SetActive(true);
    }

    public void RemoveFish(List<FishSO> lst, FishSO fish)
    {
        if (lst != fishs) return;

        if (fishs.Contains(fish))
        {
            fishs.Remove(fish);
            // 해당 물고기 오브젝트 찾아서 비활성화
            FishModel fishObj = pool.FirstOrDefault(obj => obj.gameObject.activeSelf && obj.fishSO.nameStr == fish.nameStr);
            if (fishObj != null)
            {
                fishObj.gameObject.SetActive(false);
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            base.OnTriggerEnter(other);
            GuideText.Instance.AddGuide("WaterTank");
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            base.OnTriggerExit(other);
            UIManager.Instance.fishTank.SetActive(false);
            GuideText.Instance.RemoveGuide("WaterTank");
        }
    }
    protected override void OnInterect()
    {
        InventoryManager.Instance.SetFish(50, fishs);
        UIManager.Instance.fishTank.SetActive(true);
    }
}
