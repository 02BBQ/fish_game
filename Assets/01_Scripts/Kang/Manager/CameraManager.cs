using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : SingleTon<CameraManager>
{
    private CinemachineBrain camBrain;
    public CinemachineCamera camVirtual;
    public CinemachineCamera fishingCam;
    public CinemachineBasicMultiChannelPerlin noise;

    
    private void Awake()
    {
        camBrain = GetComponent<CinemachineBrain>();
    }

    private void OnEnable()
    {
        EventManager.AddListener<CamShakeEvent>(ShakeCam);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<CamShakeEvent>(ShakeCam);
    }

    public CinemachineCamera GetCurrentVirtualCam()
    {
        return (CinemachineCamera)camBrain.ActiveVirtualCamera;
    }
    public void SetFishPriority(int value)
    {
        fishingCam.Priority = value;
    }
    void ShakeCam(CamShakeEvent camShake)
    {
        DOTween.To(() => noise.AmplitudeGain, x => noise.AmplitudeGain = x, camShake.amplitude, camShake.duration).SetEase(Ease.Linear);
        DOTween.To(() => noise.FrequencyGain, x => noise.FrequencyGain = x, camShake.frequency, camShake.duration).SetEase(Ease.Linear);
    }
}