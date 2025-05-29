using UnityEngine;
using Unity.Cinemachine;

public class CameraToggleLock : MonoBehaviour
{
    private CinemachineInputAxisController inputAxisController;
    private bool isLocked = false;

    private void Awake()
    {
        inputAxisController = GetComponent<CinemachineInputAxisController>();
        ToggleCamera(false);
    }

    private void Start()
    {
        Definder.Player.playerInput.CamereLock += HandleCameraLock;
    }

    private void OnDestroy()
    {
        Definder.Player.playerInput.CamereLock -= HandleCameraLock;
    }

    private void HandleCameraLock()
    {
        ToggleCamera(!isLocked);
    }

    private void ToggleCamera(bool v)
    {
        isLocked = v;

        inputAxisController.enabled = v;

        ToggleCursor(!v);
    }

    private void ToggleCursor(bool v)
    {
        Cursor.visible = v;
        Cursor.lockState = v ? CursorLockMode.None : CursorLockMode.Locked;
    }
}