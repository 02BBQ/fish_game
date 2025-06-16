using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Camera cam;

    [SerializeField]
    private float zoomStep = 10, minCamSize = 20, maxCamSize = 233;

    private float mapMinX, mapMaxX, mapMinZ, mapMaxZ;

    private Vector3 dragOrigin;

    private void Start()
    {
        mapMinX = cam.transform.position.x - cam.orthographicSize;
        mapMinZ = cam.transform.position.z - cam.orthographicSize;
        mapMaxX = cam.transform.position.x + cam.orthographicSize;
        mapMaxZ = cam.transform.position.z + cam.orthographicSize;
    }
    private void OnEnable()
    {
        UIManager.Instance.uiInput.MouseScroll += Scroll;
    }
    private void OnDisable()
    {
        UIManager.Instance.uiInput.MouseScroll -= Scroll;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        dragOrigin = cam.ScreenToWorldPoint(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(eventData.position);

        cam.transform.position = ClampCamera(cam.transform.position + new Vector3(difference.x, 0f, difference.z));
    }

    private void Scroll(Vector2 scroll)
    {
        float newSize = cam.orthographicSize - (scroll.y * zoomStep);
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPos)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinZ + camHeight;
        float maxY = mapMaxZ - camHeight;

        float newX = Mathf.Clamp(targetPos.x, minX, maxX);   
        float newZ = Mathf.Clamp(targetPos.z, minY, maxY);

        return new Vector3(newX, targetPos.y, newZ);
    }
}
