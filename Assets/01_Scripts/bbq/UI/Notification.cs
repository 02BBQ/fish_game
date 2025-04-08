using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class Notification : MonoBehaviour
{
    [SerializeField] private TMP_Text textBase;

    private Queue<TMP_Text> pool;

    void Awake()
    {
        pool = new();
    }

    private void OnEnable()
    {
        EventManager.AddListener<NotificationEvent>(Notify);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<NotificationEvent>(Notify);
    }

    public void Notify(NotificationEvent evt)
    {
        string data = evt.text;
        
        var text = GetBase();
        text.transform.SetSiblingIndex(0);

        text.fontSize = 0f;
        DOTween.To(
            () => text.fontSize,           // 현재 값 가져오기
            x => text.fontSize = x,        // 값 설정하기
            44f,                              // 목표 크기
            0.3f                                // 지속 시간
        ).SetEase(Ease.OutBounce);

        text.text = data;
        DOVirtual.DelayedCall(3f, () => {
            DOTween.To(
                () => text.fontSize,           // 현재 값 가져오기
                x => text.fontSize = x,        // 값 설정하기
                0f,                              // 목표 크기
                .3f                                // 지속 시간
            ).SetEase(Ease.InQuad).OnComplete(() => {
                DestroyBase(text);
            });
        }
        );
    }

    private TMP_Text GetBase()
    {
        TMP_Text v;
        if (pool.Count > 0)
        {
            v = pool.Dequeue();
            v.gameObject.SetActive(true);
            return v;
        }
        else
        {
            v = Instantiate(textBase, transform);
        }
        v.fontSize = 0;
        return v;
    }

    private void DestroyBase(TMP_Text text)
    {
        pool.Enqueue(text);
        text.gameObject.SetActive(false);
    }
}
