using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ItemInfo : MonoBehaviour //, IItemInfoVi
{
    [field: SerializeField] public TMP_Text nameText { get; private set; }
    [field: SerializeField] public TMP_Text descText { get; private set; }
    [field: SerializeField] public string modelAddress { get; private set; }

    [SerializeField] private ItemRotator modelPosition;
    private LayerMask viewLayer => LayerMask.NameToLayer("ViewModel");

    private GameObject loadedModel;
    private AsyncOperationHandle<GameObject> handle;

    private const string defaultModelAddress = "Fish/Default";

    private Item currentItem;

    private SpriteRenderer spriteModel;

    private void Start()
    {
        spriteModel = new GameObject("Sprite").AddComponent<SpriteRenderer>();
        spriteModel.transform.SetParent(modelPosition.transform);
        spriteModel.transform.localPosition = Vector3.zero;
        spriteModel.transform.localRotation = Quaternion.identity;
        spriteModel.transform.localScale *= .25f;

        spriteModel.sortingLayerName = "UI";
        spriteModel.sortingOrder = 1;

        spriteModel.gameObject.layer = viewLayer;
    }

    public void UpdateItemInfo(Item item)
    {
        if (item == null)
        {
            nameText.text = "";
            descText.text = "";
            currentItem = null;
            if (spriteModel == null)
                Start();
            spriteModel.gameObject.SetActive(false);
        }
        else
        {
            nameText.text = item.GetName();
            descText.text = item.GetDescription().ToString();
            currentItem = item;
            if (item is ModelView itemModel && item.visualPath != null && item.visualPath != string.Empty)
            {
                modelAddress = item.visualPath;
                LoadModel(modelAddress);
            }
            else
            {
                LoadSprite(item.image);
            }
        }
    }

    private void LoadSprite(Sprite image)
    {
        if (loadedModel != null)
        {
            Destroy(loadedModel);
            loadedModel = null;

            ReleaseHandle();
        }

        spriteModel.gameObject.SetActive(true);
        
        if (image != null)
        {
            spriteModel.sprite = image;

            SetLayerRecursively(loadedModel, viewLayer);
        }
        else
        {
            Debug.LogError("이미지를 로드할 수 없습니다.");
        }
    }

    private void LoadModel(string key)
    {
        if (loadedModel != null)
        {
            Destroy(loadedModel);
            loadedModel = null;

            ReleaseHandle();
        }

        spriteModel.gameObject.SetActive(false);
        
        modelAddress = key;
        
        Addressables.LoadResourceLocationsAsync(modelAddress).Completed += OnResourceLocationLoaded;
    }

    private void OnDestroy()
    {
        Destroy(spriteModel);
    }

    private void OnResourceLocationLoaded(AsyncOperationHandle<IList<IResourceLocation>> locationHandle)
    {
        if (locationHandle.Status == AsyncOperationStatus.Succeeded && locationHandle.Result.Count > 0)
        {
            handle = Addressables.LoadAssetAsync<GameObject>(modelAddress);
            handle.Completed += OnModelLoaded;
        }
        else
        {
            if (modelAddress != defaultModelAddress)
            {
                Debug.LogWarning($"모델 '{modelAddress}'을(를) 찾을 수 없습니다. 기본 모델을 로드합니다.");
                LoadModel(defaultModelAddress);
                // LoadSprite(currentItem.image);
            }
            else
            {
                // Debug.LogError("기본 모델도 찾을 수 없습니다.");
            }
        }

        Addressables.Release(locationHandle);
    }

    private void OnModelLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            loadedModel = Instantiate(obj.Result, modelPosition.transform.position, Quaternion.identity, modelPosition.transform);
            modelPosition.Reset();
            SetLayerRecursively(loadedModel, viewLayer);
        }
        else
        {
            Debug.LogError($"모델 '{modelAddress}' 로드에 실패했습니다.");
        }
    }

    private void ReleaseHandle()
    {
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }
    }

    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}