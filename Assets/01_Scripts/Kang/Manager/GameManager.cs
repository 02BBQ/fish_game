using System;
using System.Net.Sockets;
using TMPro;
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
