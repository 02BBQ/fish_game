using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Animations;

public class FishCanvas : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform bar;
    public RectTransform target;

    public RectTransform parent;

    private Image targetImage;

    [SerializeField] private Image rotator;

    private bool isRotating = false;
    private bool isFishing = true;

    Vector3 pos;

    private float originalY;
    public float hiddenOffsetY => -parent.rect.height; // UI 높이만큼 아래로 숨김

    void Start()
    {
        // hiddenOffsetY ;
        originalY = parent.anchoredPosition.y;
        parent.anchoredPosition = new Vector2(parent.anchoredPosition.x, originalY + hiddenOffsetY);
    }

    private void Update()
    {
        if (!isFishing) return;
        if (isRotating)
        {
            rotator.rectTransform.Rotate(Vector3.forward * 1000 * Time.deltaTime);
        }
        else
        {
            rotator.rectTransform.Rotate(Vector3.forward * (Mathf.PerlinNoise1D(Time.time*10f)-.5f)*2 * 300 * Time.deltaTime);
        }
    }

    public void ToggleRotator(bool obj)
    {
        isRotating = obj;
    }

    private void OnEnable()
    {
        targetImage = target.GetComponent<Image>();
    }

    public void ToggleCanvasGroup(bool obj)
    {
        canvasGroup.interactable = obj;
        canvasGroup.blocksRaycasts = obj;
        canvasGroup.alpha = obj ? 1 : 0;
        if (!obj)
        {
            ToggleRod(false); 
        }
    }

    public void SetColor(bool v)
    {
        targetImage.color = new Color(1,1,1, v ? 1 : .5f);
    }

    public void StartEvent()
    {
        Definder.Player.GetComponentInChildren<BEP>().PlayAttention();
        DOVirtual.DelayedCall(1.2f, () => {
            ToggleCanvasGroup(true);
            canvasGroup.alpha = 0;
            DOTween.To(
                () => canvasGroup.alpha,           // 현재 값 가져오기
                x => canvasGroup.alpha = x,        // 값 설정하기
                1f,                              // 목표 크기
                .5f                                // 지속 시간
            ).SetEase(Ease.OutQuad);
            DOVirtual.DelayedCall(.1f, () => 
            {
                ToggleRod(true);
            });
        });
    }

    public void ToggleRod(bool v)
    {
        float targetY = v ? originalY : originalY + hiddenOffsetY;

        parent.DOAnchorPosY(targetY, .6f).SetEase(Ease.InOutCubic);
    }

    public void EndEvent()
    {
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => ToggleCanvasGroup(false));
    }
}
