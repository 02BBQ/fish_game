using UnityEngine;

public class LookCamera : MonoBehaviour
{
    Transform _cam;
    private void Awake()
    {
        _cam = Definder.MainCam.transform;
    }
    void LateUpdate()
    {
        transform.rotation = _cam.rotation;
    }
}
