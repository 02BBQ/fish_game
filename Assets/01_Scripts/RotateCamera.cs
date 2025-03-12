using UnityEngine; // 이미지 3000 x 3000
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class RotateCamera : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Transform cam;
    public Transform player;
    public float sens; // 4 적당
    public Image shotButton;
    public Color downColor;
    public UnityEvent OnShot;
    public UnityEvent OnUnShot;
    float _xAngle = 0f;
    float _yAngle = 0f;
    float _y = 0f;
    bool clickShot = false;
    public void OnDrag(PointerEventData eventData)
    {
        _xAngle = eventData.delta.x * Time.deltaTime * sens;
        _yAngle = eventData.delta.y * Time.deltaTime * sens;
        _y += -1 * _yAngle;
        _y = Mathf.Clamp(_y, -80f, 80f);
        player.Rotate(0f, _xAngle, 0f);
        cam.localRotation = Quaternion.Euler(_y, 0f, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(((Vector3)eventData.position - shotButton.transform.position).magnitude <= shotButton.rectTransform.rect.width)
        {
            shotButton.color = downColor;
            clickShot = true;
            OnShot?.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (clickShot)
        {
            shotButton.color = Color.white;
            clickShot = false;
            OnUnShot?.Invoke();
        }
    }
}
