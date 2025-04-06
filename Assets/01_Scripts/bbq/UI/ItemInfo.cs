using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using System;

public class ItemInfo : MonoBehaviour //, IItemInfoView
{
    [field: SerializeField] public TMP_Text nameText { get; private set; }
    [field: SerializeField] public TMP_Text descText { get; private set; }
    [field: SerializeField] public string modelAddress { get; private set; }

    [SerializeField] private ItemRotator modelPosition;
    private LayerMask viewLayer => LayerMask.NameToLayer("ViewModel");

    private GameObject loadedModel;
    private AsyncOperationHandle<GameObject> handle;

    private const string defaultModelAddress = "Fish/Default";


    public void UpdateItemInfo(Item item)
    {
        
        nameText.text = item.GetName();
        descText.text = item.GetDescription().ToString();
        if (item is ModelView itemModel)
        {
            modelAddress = itemModel.addressPath;
            LoadModel(modelAddress);
        }
        else
        {
            LoadSprite(item.image);
        }
    }

    private void LoadSprite(Sprite image)
    {
        throw new NotImplementedException();
    }

    public void LoadModel(string key)
    {
        if (loadedModel != null)
        {
            Destroy(loadedModel);
            loadedModel = null;

            Addressables.Release(handle);
        }
        
        modelAddress = key;
        
        Addressables.LoadResourceLocationsAsync(modelAddress).Completed += OnResourceLocationLoaded;
    }

    private void OnResourceLocationLoaded(AsyncOperationHandle<IList<IResourceLocation>> locationHandle)
    {
        if (locationHandle.Status == AsyncOperationStatus.Succeeded && locationHandle.Result.Count > 0)
        {
            // 에셋이 존재하면 로드
            handle = Addressables.LoadAssetAsync<GameObject>(modelAddress);
            handle.Completed += OnModelLoaded;
        }
        else
        {
            // 에셋이 존재하지 않으면 기본 모델 로드
            if (modelAddress != defaultModelAddress)
            {
                Debug.LogWarning($"모델 '{modelAddress}'을(를) 찾을 수 없습니다. 기본 모델을 로드합니다.");
                LoadModel(defaultModelAddress);
            }
            else
            {
                Debug.LogError("기본 모델도 찾을 수 없습니다.");
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