using TMPro;
using UnityEngine;

public class ItemInfo : MonoBehaviour, IItemInfoView
{
    [field: SerializeField] public TMP_Text nameText { get; private set; }
    [field: SerializeField] public TMP_Text descText { get; private set; }

    // private ItemInfoPresenter _presenter;

    // private void Awake()
    // {
    //     _presenter = new ItemInfoPresenter(this, InventoryManager.Instance);
    // }

    public void UpdateItemInfo(string name, string description)
    {
        nameText.text = name;
        descText.text = description;
    }
}