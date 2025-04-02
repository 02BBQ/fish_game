using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemInfo : MonoBehaviour, IItemInfoView
{
    [field: SerializeField] public TMP_Text nameText { get; private set; }
    [field: SerializeField] public TMP_Text descText { get; private set; }
    [field: SerializeField] public string modelAddress { get; private set; }

    [SerializeField] private Transform modelPosition;
    private LayerMask viewLayer => LayerMask.NameToLayer("ViewModel");

    private GameObject loadedModel;
    private AsyncOperationHandle<GameObject> handle;


    // private ItemInfoPresenter _presenter;

    // private void Awake()
    // {
    //     _presenter = new ItemInfoPresenter(this, InventoryManager.Instance);
    // }

    public void UpdateItemInfo(string name, string description)
    {
        nameText.text = name;
        descText.text = description;
        LoadModel("Fish/Default");
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
        handle = Addressables.LoadAssetAsync<GameObject>(modelAddress);
        handle.Completed += OnModelLoaded;
    }

    private void OnModelLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            loadedModel = Instantiate(obj.Result, modelPosition.position, Quaternion.identity, modelPosition);
            SetLayerRecursively(loadedModel, viewLayer);
            // loadedModel.AddComponent<Rotator>();
        }
        else
        {
            Debug.LogError("모델 로드 실패: " + modelAddress);
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