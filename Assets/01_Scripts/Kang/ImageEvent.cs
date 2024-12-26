using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ImageEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool outlineAction;
    public bool scaleAction;
    public bool pointingAction;
    public float scale;
    public Color color;
    private Outline outline;
    Color originColor;
    Image image;
    private void Awake()
    {
        if (outlineAction)
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }
        image = GetComponent<Image>();
        originColor = image.color;
    }
    private void OnEnable()
    {
        if (outlineAction) outline.enabled = false;
        if (scaleAction) transform.localScale = Vector3.one;
        if (pointingAction) image.color = originColor;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (outlineAction) outline.enabled = true;
        if (scaleAction) transform.DOScale(new Vector3(scale, scale, 1), 0.2f).SetUpdate(true);
        if (pointingAction)
        {
            image.DOColor(color, 0.2f).SetUpdate(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outlineAction) outline.enabled = false;
        if (scaleAction) transform.DOScale(Vector3.one, 0.2f).SetUpdate(true);
        if (pointingAction) image.DOColor(originColor, 0.2f).SetUpdate(true);
    }
}
