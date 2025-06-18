using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterTank : Interactor
{
    public List<FishSO> fishDefinitions; // FishSO 스크립터블 오브젝트 리스트
    public FishModel fishPrefab; // 물고기 프리팹
    List<FishModel> fishPool; // 오브젝트 풀


    private void Awake()
    {
        fishPool = new List<FishModel>();

        // Steam 인벤토리 초기화 확인
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam not initialized!");
            return;
        }
    }


    protected override void Start()
    {
        base.Start();
        RefreshFishTank(); // 초기 물고기 로드
    }

    // Steam 인벤토리 변경 시 호출
    private void RefreshFishTank()
    {
        // 기존 물고기 비활성화
        foreach (var fish in fishPool)
        {
            fish.gameObject.SetActive(false);
        }

    /*    // Steam 인벤토리에서 물고기 아이템 조회
        foreach (FishSO fishDef in fishDefinitions)
        {
            SteamItemDef_t itemDef = new SteamItemDef_t(fishDef.price);

            if (SteamInventoryManager.Instance.TryGetItemDetails(itemDef, out var itemDetails))
            {
                // 보유 수량만큼 물고기 생성
                for (int i = 0; i < itemDetails.m_unQuantity; i++)
                {
                    AddFishToTank(fishDef);
                }
            }
        }*/
    }

    private void AddFishToTank(FishSO fishData)
    {
        // 풀에서 재사용 가능한 물고기 찾기
        FishModel fishObj = fishPool.FirstOrDefault(obj => !obj.gameObject.activeSelf);

        if (fishObj == null)
        {
            // 새 오브젝트 생성
            fishObj = Instantiate(fishPrefab, transform);
            fishPool.Add(fishObj);
        }

        fishObj.Init(fishData.image, fishData);
        fishObj.gameObject.SetActive(true);
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

    protected override void OnInteract()
    {
        // 물고기 탭 UI 열기
        UIManager.Instance.fishTank.SetActive(true);

        // 인벤토리 새로고침 (UI에 최신 데이터 반영)
        //SteamInventoryManager.Instance.RefreshInventory();
    }
}