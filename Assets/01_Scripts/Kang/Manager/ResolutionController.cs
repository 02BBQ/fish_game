using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{
    FullScreenMode screenMode;
    public ArrowUI resolutionBtn;
    public ArrowUI fullscreenBtn;
    List<Resolution> resolutions = new List<Resolution>();
    int resolutionNum;
    void Start()
    {
        InitUI();
    }
    void InitUI()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            resolutions.Add(Screen.resolutions[i]);
        }
        resolutionBtn.options.Clear();

        foreach (Resolution item in resolutions)
        {
            string option;
            option = $"{item.width}x{item.height} {(int)item.refreshRateRatio.value}hz";
            resolutionBtn.options.Add(option);

        }
        Apply();

    }
    public void Apply()
    {
        fullscreenBtn.SetJustValue(JsonManager.Instance.FullScreen);
        screenMode = fullscreenBtn.Index == 0 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        resolutionNum = JsonManager.Instance.Resolution;
        resolutionBtn.SetJustValue(resolutionNum);
        Refresh();
    }
    public void DropboxOptionChange(int x)
    {
        JsonManager.Instance.Resolution = x;
        resolutionNum = x;
        Refresh();
    }
    public void FullScreenBtn(int isFull)
    {
        JsonManager.Instance.FullScreen = isFull;
        screenMode = isFull == 0 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Refresh();
    }
    public void Refresh()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode, resolutions[resolutionNum].refreshRateRatio);
    }
}
