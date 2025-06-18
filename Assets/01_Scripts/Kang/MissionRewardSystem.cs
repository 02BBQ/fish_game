using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class MissionRewardSystem : MonoBehaviour
{
    [Header("Settings")]
    public int rewardAmount = 50;
    public float rewardCooldownHours = 5f;
    public string rewardSaveKey = "LastMissionRewardTime";

    [Header("Visuals")]
    public TextMeshPro rewardStatusText;
    public GameObject rewardParticle;

    private Trigger _trigger;
    private DateTime _lastRewardTime;
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
        string savedTime = PlayerPrefs.GetString(rewardSaveKey, "");
        _lastRewardTime = string.IsNullOrEmpty(savedTime) ? DateTime.MinValue : DateTime.Parse(savedTime);
    }

    private void UpdateRewardStatus()
    {
        TimeSpan timeSinceLastReward = DateTime.Now - _lastRewardTime;
        _canClaimReward = timeSinceLastReward.TotalHours >= rewardCooldownHours;

        if (_canClaimReward)
        {
            rewardStatusText.text = "Reward Ready!";
        }
        else
        {
            TimeSpan timeLeft = TimeSpan.FromHours(rewardCooldownHours) - timeSinceLastReward;

            // 5�ð� �̳��� ��츸 ���� �ð� ǥ��
            if (timeLeft.TotalHours > 0)
            {
                rewardStatusText.text = string.Format("{0:00}:{1:00}:{2:00}",
                    (int)timeLeft.TotalHours, timeLeft.Minutes, timeLeft.Seconds);
            }
            else
            {
                rewardStatusText.text = "Reward Ready!";
            }
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

            // ���� �ð� ǥ�� (�ɼ�)
            TimeSpan timeLeft = TimeSpan.FromHours(rewardCooldownHours) - (DateTime.Now - _lastRewardTime);
            Debug.Log($"���� ������� ���� �ð�: {timeLeft.Hours}�ð� {timeLeft.Minutes}��");
        }
    }

    private void ClaimReward()
    {
        _lastRewardTime = DateTime.Now;
        PlayerPrefs.SetString(rewardSaveKey, _lastRewardTime.ToString());
        PlayerPrefs.Save();

        rewardParticle.SetActive(true);
        //@���� ������ ����

        UpdateRewardStatus();
    }
}