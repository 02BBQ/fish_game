using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class DailyRewardSystem : MonoBehaviour
{
    [Header("Settings")]
    public int rewardAmount = 100;
    public string rewardSaveKey = "LastRewardDate";

    [Header("Visuals")]
    public TextMeshPro rewardStatusText;
    public GameObject rewardParticle;

    private Trigger _trigger;
    private DateTime _lastRewardDate;
    private bool _canClaimReward = false;

    private void Awake()
    {
        _trigger = GetComponent<Trigger>();
        LoadRewardData();
    }

    private void OnEnable()
    {
        _trigger.TriggerEnter.AddListener(OnPlayerTriggerEnter);
    }

    private void OnDisable()
    {
        _trigger.TriggerEnter.RemoveListener(OnPlayerTriggerEnter);
    }

    private void Update()
    {
        UpdateRewardStatus();
    }
    
    [ContextMenu("Reset")]
    public void RemoveMemeory()
    {
        PlayerPrefs.DeleteKey(rewardSaveKey);
    }

    private void LoadRewardData()
    {
        string savedDate = PlayerPrefs.GetString(rewardSaveKey, "");
        _lastRewardDate = string.IsNullOrEmpty(savedDate) ? DateTime.MinValue : DateTime.Parse(savedDate);
    }

    private void UpdateRewardStatus()
    {
        DateTime now = DateTime.Now;
        DateTime nextResetTime = _lastRewardDate.Date.AddDays(1);

        // 보상 수령 가능 여부 확인
        _canClaimReward = now >= nextResetTime;

        if (_canClaimReward)
        {
            rewardStatusText.text = "Reward Ready!";
        }
        else
        {
            TimeSpan timeLeft = nextResetTime - now;
            rewardStatusText.text = string.Format("{0:00}:{1:00}:{2:00}",
                timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
        }
    }

    private void OnPlayerTriggerEnter(Collider player)
    {
        if (!player.CompareTag("Player")) return;

        if (_canClaimReward)
        {
            ClaimReward();
        }
        else
        {
            Debug.Log("아직 보상을 받을 수 없습니다.");
        }
    }

    private void ClaimReward()
    {
        _lastRewardDate = DateTime.Now;
        PlayerPrefs.SetString(rewardSaveKey, _lastRewardDate.ToString());
        PlayerPrefs.Save();

        // 보상 지급
        Definder.GameManager.moneyController.EarnMoney(rewardAmount);
        rewardParticle.SetActive(true);

        // 효과음 등 추가 효과 구현 가능
        Debug.Log($"일일 보상 {rewardAmount} 코인 획득!");

        // 상태 업데이트
        UpdateRewardStatus();
    }
}