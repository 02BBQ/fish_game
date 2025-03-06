using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : SingleTon<CameraManager>
{
    private CinemachineBrain camBrain;
    public CinemachineCamera camVirtual;

    private void Awake()
    {
        camBrain = GetComponent<CinemachineBrain>();
    }
    public CinemachineCamera GetCurrentVirtualCam()
    {
        return (CinemachineCamera)camBrain.ActiveVirtualCamera;
    }
}
