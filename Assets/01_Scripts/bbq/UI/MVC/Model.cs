
using System;
using UnityEngine;

[System.Serializable]
public class MoneyModel
{
    public int CurrentMoney => _money;
    
    [SerializeField] private int _money = 1000;
    public event Action<int> OnMoneyChanged;

    public void SetMoney(int amount)
    {
        _money = amount;
        OnMoneyChanged?.Invoke(_money);
    }

    public void AddMoney(int amount)
    {
        _money += amount;
        OnMoneyChanged?.Invoke(_money);
    }

    public void SpendMoney(int amount)
    {
        _money -= amount;
        OnMoneyChanged?.Invoke(_money);
    }
}