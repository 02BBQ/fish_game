using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum Dir : short
{
    x,
    y
}
[Serializable]
public struct UI
{
    public RectTransform changeUI;
    public CanvasGroup fadeUI;
    public Dir dir;
    public Vector2 inAndOut;
    public float time;
    public bool setActive;
    public float fadeFloat;
}
public class UIManager : SingleTon<UIManager>
{
    public UI[] settingUI;
    public UI[] mainUI;
    public UI[] playUI;
    public UI[] pauseUI;
    public GameObject block;
    public GameObject boatLabelPref;
    public Transform labelParent;
    public TextMeshProUGUI oceanText;
    public Image playerIcon;
    public List<string> oceanNames;
    public UIInput uiInput;

    public GameObject dock;
    public GameObject fishTank;
    [SerializeField] private Image image;
    [SerializeField] private GameObject map;

    private Tween currentTween;
    int currentOcean;

    #region UNITY_EVENT
    private void Start()
    {
        currentOcean = Definder.Player.GetCurrentOcean();
    }
    private void OnEnable()
    {
        uiInput.OnClickESC += Definder.GameManager.PauseGame;
        uiInput.OnClickMap += ToggleMap;
        
    }
    private void OnDisable()
    {
        uiInput.OnClickESC -= Definder.GameManager.PauseGame;
        uiInput.OnClickMap -= ToggleMap;
    }
    private void Update()
    {
        if(uiInput.leftClicked)
            uiInput.OnLeft?.Invoke();

        int frameOcean = Definder.Player.GetCurrentOcean();
        if (currentOcean != frameOcean)
        {
            if (PostText(frameOcean))
            {
                currentOcean = frameOcean;
            }
        }
    }
    #endregion
    private void ToggleMap()
    {
        if(Definder.GameManager.startGame)
            map.SetActive(!map.activeSelf);
    }

    private bool PostText(int index)
    {
        if (oceanText.gameObject.activeSelf == true) return false;

        oceanText.gameObject.SetActive(true);
        oceanText.SetText(oceanNames[index-1]);
        oceanText.DOFade(1f, 0.7f).OnComplete(() =>
        {
            oceanText.DOFade(0f, 0.7f).SetDelay(1.5f).OnComplete(() =>
            {
                oceanText.gameObject.SetActive(false);
            });
        });
        return true;
    }

    public void SettingUIIn()
    {
        In(settingUI);
    }
    public void SettingUIOut()
    {
        Out(settingUI);
    }
    public void MainUIIn()
    {
        In(mainUI);
    }

    public void MainUIOut()
    {
        Out(mainUI);
    }
    public void PlayUIIn()
    {
        In(playUI);
    }
    public void PlayUIOut()
    {
        Out(playUI);
    }
    public void PauseUIIn()
    {
        In(pauseUI);
    }
    public void PauseUIOut()
    {
        Out(pauseUI);
    }
    private void In(UI[] lst)
    {
        block.SetActive(true);
        float max = 0;
        for (int i = 0; i < lst.Length; i++)
        {
            UI currentLst = lst[i];

            if (max < currentLst.time) max = currentLst.time;

            if (currentLst.changeUI != null)
            {
                if (currentLst.setActive)
                {
                    currentLst.changeUI.gameObject.SetActive(true);
                }
                if (currentLst.dir == Dir.y) 
                    currentLst.changeUI.DOAnchorPosY(currentLst.inAndOut.x, currentLst.time).SetUpdate(true).SetEase(Ease.Linear);
                else 
                    currentLst.changeUI.DOAnchorPosX(currentLst.inAndOut.x, currentLst.time).SetUpdate(true).SetEase(Ease.Linear);
            }
            else if (currentLst.fadeUI != null)
            {
                if (currentLst.setActive)
                {
                    currentLst.fadeUI.gameObject.SetActive(true);
                    currentLst.fadeUI.interactable = true;
                    currentLst.fadeUI.blocksRaycasts = true;
                }
                currentLst.fadeUI.DOFade(currentLst.fadeFloat / 255f, currentLst.time).SetUpdate(true).SetEase(Ease.Linear);
            }
        }
        StartCoroutine(BlockTime(max));
    }

    private void Out(UI[] lst)
    {
        block.SetActive(true);
        float max = 0;
        for (int i = 0; i < lst.Length; i++)
        {
            UI currentLst = lst[i];

            if (max < currentLst.time) max = currentLst.time;
            
            UI lambdaUI = currentLst;
            if (currentLst.changeUI != null)
            {
                if (currentLst.dir == Dir.y)
                {
                    currentLst.changeUI.DOAnchorPosY(currentLst.inAndOut.y, currentLst.time).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
                    {
                        if (lambdaUI.setActive)
                        {
                            lambdaUI.changeUI.gameObject.SetActive(false);
                        }
                    });
                }
                else
                {
                    currentLst.changeUI.DOAnchorPosX(currentLst.inAndOut.y, currentLst.time).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
                    {
                        if (lambdaUI.setActive)
                        {
                            lambdaUI.changeUI.gameObject.SetActive(false);
                        }
                    });
                }
            }
            else if (currentLst.fadeUI != null)
            {
                currentLst.fadeUI.DOFade(0, currentLst.time).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
                {
                    if (lambdaUI.setActive)
                    {
                        lambdaUI.fadeUI.gameObject.SetActive(false);
                        lambdaUI.fadeUI.interactable = false;
                        lambdaUI.fadeUI.blocksRaycasts = false;
                    }
                });
            }
        }
        StartCoroutine(BlockTime(max));
    }
    IEnumerator BlockTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        block.SetActive(false);
    }

    public void FadeOut(float duration)
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        currentTween = image.DOFade(0f, duration).OnComplete(() =>
        {
            image.enabled = false;
        });
    }

    public void FadeIn(float duration)
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        image.enabled = true;
        currentTween = image.DOFade(1f, duration);
    }

    internal void MakeBoatLabel(Item index, BoatController gameObject)
    {
        BoatLabel label = Instantiate(boatLabelPref, labelParent).GetComponent<BoatLabel>();
        label.Init(index, gameObject);
    }
}