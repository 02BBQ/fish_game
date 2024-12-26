using UnityEngine;

public class FishingMovementBackUp : MonoBehaviour
{
    public RectTransform rectTrm => transform as RectTransform;

    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _moveAmplitude = 50f;
    [SerializeField] private float _noisePower = 1f;
    [SerializeField] private float _rotationAmplitude = 15f;

    private float timeX, timeY, timeR;

    private void Awake() 
    {
        timeX = Random.Range(0f, 1000f);
        timeY = Random.Range(0f, 1000f);
        timeR = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        // 현재 RectTransform의 초기 anchoredPosition (캔버스에서의 위치)
        Vector2 basePos = rectTrm.anchoredPosition;

        // Perlin Noise로 x, y 변위 계산
        float offsetX = (Mathf.PerlinNoise(timeX, 0f) - 0.5f) * 2f * _moveAmplitude;
        float offsetY = (Mathf.PerlinNoise(timeY, 1f) - 0.5f) * 2f * _moveAmplitude;
        Vector2 offset = new Vector2(offsetX, offsetY);

        // 새 anchoredPosition = 원래 위치 + 노이즈 오프셋
        Vector2 newPos = basePos + offset;

        rectTrm.anchoredPosition = newPos;

        // 회전도 노이즈로 제어 (부드럽게 흔들리는 회전)
        float rotationOffset = (Mathf.PerlinNoise(timeR, 2f) - 0.5f) * 2f * _rotationAmplitude;
        float newRotZ = rotationOffset;  // Z축(2D UI)이 회전축
        rectTrm.localRotation = Quaternion.Euler(0f, 0f, newRotZ);

        // 노이즈 시간값 업데이트
        timeX += Time.deltaTime * _moveSpeed;
        timeY += Time.deltaTime * _moveSpeed;
        timeR += Time.deltaTime * _moveSpeed;
    }
}
