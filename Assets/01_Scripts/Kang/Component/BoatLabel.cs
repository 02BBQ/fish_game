using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoatLabel : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI destriptionText;
    public TextMeshProUGUI buttonText;
    public Image icon;
    [HideInInspector] public Item item;
    [HideInInspector] public BoatController boat;
    public void Init(Item item, BoatController boat)
    {
        this.item = item;
        if (item.type != ItemType.Boat)
            return;

        nameText.text = item.nameStr;
        destriptionText.text = item.description;
        icon.sprite = item.image;
        buttonText.text = "보관";
        this.boat = boat;
    }
    public void OnClickToggle()
    {
        if (boat.gameObject.activeSelf)
        {
            boat.gameObject.SetActive(false);
            buttonText.text = "사용";
        }
        else
        {
            boat.gameObject.SetActive(true);
            buttonText.text = "보관";
        }
    }
    public void OnClickAlign()
    {
        boat.ResetPos();
    }
}
