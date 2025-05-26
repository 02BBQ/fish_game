using System;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

[RequireComponent(typeof(Trigger))]
public class DeilyCheck : MonoBehaviour
{
    Trigger trigger;
    private DateTime lastRewardDate;
    public GameObject particle;
    public TextMeshPro timerText;
    private void Awake()
    {
        trigger = GetComponent<Trigger>();
    }
    private void Start()
    {
        string savedDate = PlayerPrefs.GetString("LastRewardDate", "");
        if (!string.IsNullOrEmpty(savedDate))
        {
            lastRewardDate = DateTime.Parse(savedDate);
        }
        else
        {
            lastRewardDate = DateTime.MinValue;
        }
    }
    private void Update()
    {
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        DateTime now = DateTime.Now;
        if (lastRewardDate.Date != now.Date)
            now = lastRewardDate;
        else
            now = DateTime.Now;
        DateTime nextReset = now.Date.AddDays(1); // ���� 00:00

        TimeSpan timeLeft = nextReset - now;

        string formatted = string.Format("{0:00}:{1:00}:{2:00}",
            Mathf.Max(0, timeLeft.Hours), Mathf.Max(0, timeLeft.Minutes), Mathf.Max(0, timeLeft.Seconds));

        timerText.text = formatted;
    }
    private void OnEnable()
    {
        trigger.TriggerEnter.AddListener(CheckDeily);
    }
    private void OnDisable()
    {
        trigger.TriggerEnter.RemoveListener(CheckDeily);
    }
    private void CheckDeily(Collider arg0)
    {
        if (!arg0.CompareTag("Player")) return;
        DateTime now = DateTime.Now;

        // ��¥�� �ٸ��� ���� ����
        if (lastRewardDate.Date != now.Date)
        {
            
            lastRewardDate = now;
            particle.SetActive(true);
            Definder.GameManager.moneyController.EarnMoney(100);
            PlayerPrefs.SetString("LastRewardDate", lastRewardDate.ToString());
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("�̹� ���� ������ �޾ҽ��ϴ�.");
        }
    }
}
