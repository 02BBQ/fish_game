using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishSlot : MonoBehaviour, IPointerDownHandler
{
    Image image;
    public Image childImage;
    public GameObject child;
    public Color selectedColor, normalColor;
    [field: SerializeField] public FishSO slotItem;

    [SerializeField] bool isPersonal = false;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    private void Start()
    {
        if(isPersonal)
            InventoryManager.Instance.UpdateInfo(null);
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }
    public void Deselect()
    {
        image.color = normalColor;
    }

    public void ResetItem()
    {
        slotItem = null;
        childImage.sprite = null;
        child.SetActive(false);
    }
    public void SetItem(FishSO item)
    {
        slotItem = item;
        childImage.sprite = item.image;
        child.SetActive(true);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        InventoryManager.Instance.UpdateInfo(slotItem);
        
        if(isPersonal)
            InventoryManager.Instance.OnClickPersonalFish();
        else
            InventoryManager.Instance.OnClickFish();
        
        InventoryManager.Instance.clickedFish = this;

        if (slotItem == null)
            InventoryManager.Instance.ResetFishButtons();
    }
}
