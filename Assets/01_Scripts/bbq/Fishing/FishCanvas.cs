using UnityEngine;
using DG.Tweening;

public class FishCanvas : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform bar;
    public RectTransform target;

    public void ToggleCanvasGroup(bool obj)
    {
        canvasGroup.interactable = obj;
        canvasGroup.blocksRaycasts = obj;
        canvasGroup.alpha = obj ? 1 : 0;
    }

    public void StartEvent()
    {
        canvasGroup.alpha = 0;
        ToggleCanvasGroup(true);
        canvasGroup.DOFade(1, 0.5f);
    }

    public void EndEvent()
    {
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => ToggleCanvasGroup(false));
    }
}
