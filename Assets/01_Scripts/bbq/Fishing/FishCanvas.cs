using UnityEngine;

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
}
