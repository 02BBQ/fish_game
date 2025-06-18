using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class ModelLoader : MonoBehaviour
{
    private const float UNLOAD_DELAY = 10f;
    private const string defaultModelAddress = "Fish/Default";
    
    private GameObject loadedModel;
    private AsyncOperationHandle<GameObject> handle;
    private Coroutine unloadCoroutine;
    private bool isHovering;
    private string currentModelKey;
    private Transform modelParent;

    private void Awake()
    {
        modelParent = transform;
    }

    public void LoadModel(string key)
    {
        if (currentModelKey == key && loadedModel != null) 
            return;

        ClearPreviousResources();
        currentModelKey = key;
        
        Addressables.LoadResourceLocationsAsync(key).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
            {
                LoadModelAsync(key);
            }
            else
            {
                Debug.LogWarning($"모델 '{key}'를 찾을 수 없습니다. 기본 모델 로드 시도...");
                LoadModelAsync(defaultModelAddress);
            }
            Addressables.Release(handle);
        };
    }

    private void LoadModelAsync(string key)
    {
        handle = Addressables.LoadAssetAsync<GameObject>(key);
        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded && isHovering)
            {
                loadedModel = Instantiate(op.Result, modelParent);
                loadedModel.transform.localPosition = Vector3.zero;
                SetLayerRecursively(loadedModel, LayerMask.NameToLayer("ViewModel"));
            }
            else if (op.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"모델 로드 실패: {key}");
            }
        };
    }

    public void OnHoverStart()
    {
        isHovering = true;
        CancelPendingUnload();
    }

    public void OnHoverEnd()
    {
        isHovering = false;
        unloadCoroutine = StartCoroutine(DelayedUnload());
    }

    private IEnumerator DelayedUnload()
    {
        yield return new WaitForSeconds(UNLOAD_DELAY);
        
        if (!isHovering)
        {
            ClearPreviousResources();
        }
    }

    private void CancelPendingUnload()
    {
        if (unloadCoroutine != null)
        {
            StopCoroutine(unloadCoroutine);
            unloadCoroutine = null;
        }
    }

    private void ClearPreviousResources()
    {
        if (loadedModel != null)
        {
            var renderers = loadedModel.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer.materials != null)
                {
                    foreach (var mat in renderer.materials)
                    {
                        if (mat != null && mat.name.Contains("(Instance)"))
                        {
                            DestroyImmediate(mat);
                        }
                    }
                }

                var meshFilter = renderer.GetComponent<MeshFilter>();
                var skinnedMesh = renderer.GetComponent<SkinnedMeshRenderer>();
                
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    Resources.UnloadAsset(meshFilter.sharedMesh);
                }
                if (skinnedMesh != null && skinnedMesh.sharedMesh != null)
                {
                    Resources.UnloadAsset(skinnedMesh.sharedMesh);
                }
            }

            Destroy(loadedModel);
            loadedModel = null;
        }
        
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }
        
        currentModelKey = null;
    }

    private void OnDestroy()
    {
        CancelPendingUnload();
        ClearPreviousResources();
        StopAllCoroutines();
    }

    private static void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
} 