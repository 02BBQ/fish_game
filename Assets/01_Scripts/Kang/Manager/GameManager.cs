using System;
using System.Net.Sockets;
using ServerData;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

namespace ServerData
{
    [System.Serializable]
    public class InventoryItemData
    {
        public string guid;
        public string Address;
        public string Category;
        public int? CurrencyCount;
        public string Desc;
        public int Id;
        public int Limit;
        public int MaxPerPurchase;
        public string Name;
        public bool Stackable;
        // public ServerItem item;
    }

    [System.Serializable]
    public class ServerResponse
    {
        public int money;
        public InventoryData inventoryData;
    }
}
public class GameManager : MonoBehaviour
{
    int _coin;
    public int Coin { 
        get => _coin;
        set
        {
            _coin = value;
            SetCoinText();
        }
    }

    [SerializeField] private TextMeshProUGUI coinText;
    public Transform spawnPoint;
    public bool startGame = false;

    private void handleFishJson(InitData data)
    {
        Coin = data.money;
        if (data.inventoryData.Fish != null)
        {
            foreach (FishJson fish in data.inventoryData.Fish)
            {
                FishSO so = ScriptableObject.CreateInstance<FishSO>();
                so.Initialize(fish);
                InventoryManager.Instance.AddItem(so);
            }
            foreach (InventoryItemData fish in data.inventoryData.FishingRod)
            {
                // Load fishing rod prefab from addressables
                var handle = Addressables.LoadAssetAsync<FishingRod>(fish.Address);
                handle.Completed += (op) =>
                {
                    if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        var fishingRod = op.Result;
                        if (fishingRod != null)
                        {
                            // fishingRod.Initialize(fish);
                            InventoryManager.Instance.AddItem(fishingRod);
                        }
                        else
                        {
                            Debug.LogError($"FishingRod component not found on prefab: {fish.Address}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to load fishing rod from address: {fish.Address}");
                    }
                };
            }
        }
        else
        {
            Debug.Log("No fish data available.");
        }
    }

    private void Start()
    {
        FishingServerConnector.Instance.GetData("test", handleFishJson);
        Time.timeScale = 1f;
        SetCoinText();
    }

    [ContextMenu("dsfa")]
    public void Delete()
    {
        PlayerPrefs.DeleteAll();
    }
    public void OnClickStart()
    {
        EventBus.Publish(EventBusType.Start);
        Definder.Player.playerMovement.movable = true;
        startGame = true;
        UIManager.Instance.PlayUIIn();
        UIManager.Instance.MainUIOut();
    }
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();  // �����Ϳ��� ���� ���̸� Play ��� ����
#else
        Application.Quit();  // ����� ���ӿ����� ���� ����
#endif
    }
    private void SetCoinText()
    {
        coinText.text = _coin.ToString("0");
    }

    public void PauseGame()
    {
        if (!startGame) return; 

        Time.timeScale = 0f;
        UIManager.Instance.PauseUIIn();
        SoundManager.Instance.Pause();
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        UIManager.Instance.PauseUIOut();
        SoundManager.Instance.Unpause();
    }
    public void ReloadScene()
    {
        SceneManager.LoadSceneAsync(gameObject.scene.name);
    }
}
