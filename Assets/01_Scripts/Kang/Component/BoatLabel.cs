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
    [HideInInspector] public bool boatActive = false;
    public void Init(Item item, BoatController boat)
    {
        this.item = item;
        if (item.type != ItemType.Boat)
            return;

        nameText.text = item.nameStr;
        destriptionText.text = item.description;
        icon.sprite = item.image;
        buttonText.text = "보관";
        boatActive = true;
        this.boat = boat;
        BoatManager.Instance.labels.Add(this);
    }
    private void OnDestroy()
    {
        if(BoatManager.Instance != null)
            BoatManager.Instance.labels.Remove(this);
    }
    public void OnClickToggle()
    {
        if (boat.gameObject.activeSelf)
        {
            boat.gameObject.SetActive(false);
            boatActive = false;
            buttonText.text = "사용";
        }
        else
        {
            boat.gameObject.SetActive(true);
            boatActive = true;
            buttonText.text = "보관";
        }
    }
}
