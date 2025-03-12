using System;
using System.Net.Sockets;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int _coin;
    public int Coin { 
        get => _coin;
        set
        {
            _coin = value;
            SetCoinText();
        }
    }

    [SerializeField] private TextMeshProUGUI coinText;
    public GameObject[] boats;
    public Transform spawnPoint;

    private void Start()
    {
        Coin = 100000;
        SetCoinText();
    }
    public void OnClickStart()
    {
        EventBus.Publish(EventBusType.Start);
        Definder.Player.playerMovement.movable = true;
        UIManager.Instance.PlayUIIn();
        UIManager.Instance.MainUIOut();
    }
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();  // �����Ϳ��� ���� ���̸� Play ��� ����
#else
        Application.Quit();  // ����� ���ӿ����� ���� ����
#endif
    }
    private void SetCoinText()
    {
        coinText.text = _coin.ToString("0");
    }

    public void UnlockBoat(string index)
    {
        if(int.TryParse(index, out int result))
        {
            boats[result].SetActive(true);
        }
    }
}
