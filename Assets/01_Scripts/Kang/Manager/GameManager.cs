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
    public GameObject[] boats;
    public Transform spawnPoint;
    public bool startGame = false;

    private void Start()
    {
        Time.timeScale = 1f;
        Coin = 100000;
        SetCoinText();
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
        EditorApplication.ExitPlaymode();  // 에디터에서 실행 중이면 Play 모드 종료
#else
        Application.Quit();  // 빌드된 게임에서는 정상 종료
#endif
    }
    private void SetCoinText()
    {
        coinText.text = _coin.ToString("0");
    }

    public void UnlockBoat(string index)
    {
        if(int.TryParse(index, out int result))
        {
            boats[result].SetActive(true);
        }
    }
    public void PauseGame()
    {
        if (!startGame) return; 

        Time.timeScale = 0f;
        UIManager.Instance.PauseUIIn();
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        UIManager.Instance.PauseUIOut();
    }
    public void ReloadScene()
    {
        SceneManager.LoadSceneAsync(gameObject.scene.name);
    }
}
