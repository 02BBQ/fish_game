
using System.Data;
using UnityEngine;

public class MoneyController : MonoBehaviour
{
    [Header("MVC Components")]
    [SerializeField] private MoneyModel model;
    [SerializeField] private MoneyView view;

    private void Awake()
    {
        if (model == null)
        {
            model = new();
        }
        model.OnMoneyChanged += view.UpdateMoneyDisplay;
        view.UpdateMoneyDisplay(model.CurrentMoney);
    }

    public void SetMoney(int amount) => model.SetMoney(amount);
    public void EarnMoney(int amount) => model.AddMoney(amount);
    
    public void TrySpendMoney(int amount) => model.SpendMoney(amount);
    
    private void OnDestroy()
    {
        model.OnMoneyChanged -= view.UpdateMoneyDisplay;
        model = null;
    }
}