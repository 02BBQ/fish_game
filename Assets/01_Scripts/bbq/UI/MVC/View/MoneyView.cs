using TMPro;
using UnityEngine;

public class MoneyView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    
    public void UpdateMoneyDisplay(int amount)
    {
        moneyText.text = $"{amount:N0}";
    }
}