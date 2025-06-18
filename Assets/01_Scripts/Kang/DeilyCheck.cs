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

        // ���� ���� ���� ���� Ȯ��
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
            Debug.Log("���� ������ ���� �� �����ϴ�.");
        }
    }

    private void ClaimReward()
    {
        _lastRewardDate = DateTime.Now;
        PlayerPrefs.SetString(rewardSaveKey, _lastRewardDate.ToString());
        PlayerPrefs.Save();

        // ���� ����
        Definder.GameManager.moneyController.EarnMoney(rewardAmount);
        rewardParticle.SetActive(true);

        // ȿ���� �� �߰� ȿ�� ���� ����
        Debug.Log($"���� ���� {rewardAmount} ���� ȹ��!");

        // ���� ������Ʈ
        UpdateRewardStatus();
    }
}