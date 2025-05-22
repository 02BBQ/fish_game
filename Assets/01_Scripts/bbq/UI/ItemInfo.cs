using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private ItemRotator modelPosition;
    
    private const float UNLOAD_DELAY = 10f; // 10초 딜레이로 변경
    private const string defaultModelAddress = "Fish/Default";
    
    private GameObject loadedModel;
    private AsyncOperationHandle<GameObject> handle;
    private Coroutine unloadCoroutine;
    private bool isHovering;
    private string currentModelKey;

    public void UpdateItemInfo(Item item)
    {
        // 현재 호버 상태 갱신
        isHovering = true;
        
        // 기존 언로드 코루틴 취소
        CancelPendingUnload();

        // 기본 정보 업데이트
        nameText.text = item.GetName();
        descText.text = item.GetDescription().ToString();

        // 모델/이미지 로드 결정
        if (item is ModelView itemModel && !string.IsNullOrEmpty(item.visualPath))
        {
            LoadModel(item.visualPath);
        }
        else
        {
            LoadSprite(item.image);
        }
    }

    private void LoadModel(string key)
    {
        // 동일한 모델 이미 로드된 경우 스킵
        if (currentModelKey == key && loadedModel != null) 
            return;

        // 기존 리소스 정리
        ClearPreviousResources();

        // 새 모델 로드 시작
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
        // 어드레서블 비동기 로드
        handle = Addressables.LoadAssetAsync<GameObject>(key);
        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded && isHovering)
            {
                loadedModel = Instantiate(op.Result, modelPosition.transform);
                loadedModel.transform.localPosition = Vector3.zero;
                modelPosition.Reset();
                SetLayerRecursively(loadedModel, LayerMask.NameToLayer("ViewModel"));
            }
            else if (op.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"모델 로드 실패: {key}");
            }
        };
    }

    public void OnHoverEnd()
    {
        isHovering = false;
        unloadCoroutine = StartCoroutine(DelayedUnload());
    }

    private IEnumerator DelayedUnload()
    {
        yield return new WaitForSeconds(UNLOAD_DELAY);
        
        if (!isHovering) // 다시 호버 상태가 아닐 때만 정리
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
        // 모델 제거
        if (loadedModel != null)
        {
            var renderers = loadedModel.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                // 머티리얼 해제 (인스턴스된 경우)
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

                // 메시 해제 (스킨드 메시 포함)
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
        
        // 어드레서블 핸들 해제
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }
        
        currentModelKey = null;
    }

    private void LoadSprite(Sprite image)
    {
        // 스프라이트 구현 부분 (기존 코드 유지)
    }

    private void OnDestroy()
    {
        CancelPendingUnload();
        ClearPreviousResources();
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