using System;
using System.Net.Sockets;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void handleFishJson(InventoryData data)
    {
        foreach (FishJson fish in data.fishes)
        {
            FishSO so = ScriptableObject.CreateInstance<FishSO>();
            so.Initialize(fish);
            InventoryManager.Instance.AddItem(so);
        }
        foreach (FishJson fish in data.fishes)
        {
            FishSO so = ScriptableObject.CreateInstance<FishSO>();
            so.Initialize(fish);
            InventoryManager.Instance.AddItem(so);
        }
    }

    private void Start()
    {
        FishingServerConnector.Instance.GetData("test", handleFishJson);
        Time.timeScale = 1f;
        Coin = 100000;
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
