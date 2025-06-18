using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ItemInfo : MonoBehaviour //, IItemInfoVi
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private ModelLoader modelLoader;
    [SerializeField] private Image itemImage;
    
    public void UpdateItemInfo(Item item)
    {
        // 기본 정보 업데이트
        nameText.text = item.GetName();
        descText.text = item.GetDescription().ToString();

        // 모델/이미지 로드 결정
        if (item is ModelView itemModel && !string.IsNullOrEmpty(item.visualPath))
        {
            modelLoader.OnHoverStart();
            modelLoader.LoadModel(item.visualPath);
            itemImage.gameObject.SetActive(false);
        }
        else
        {
            modelLoader.OnHoverEnd();
            LoadSprite(item.image);
            itemImage.gameObject.SetActive(true);
        }
    }

    private void LoadSprite(Sprite image)
    {
        if (image != null)
        {
            itemImage.sprite = image;
        }
    }

    private void OnDestroy()
    {
        modelLoader.OnHoverEnd();
    }
}