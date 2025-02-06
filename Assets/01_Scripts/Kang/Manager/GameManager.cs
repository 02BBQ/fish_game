using System;
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

    private void Start()
    {
        SetCoinText();
    }

    private void SetCoinText()
    {
        coinText.text = _coin.ToString("0");
    }
}
