using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

public class FishTray : MonoBehaviour
{
    [Header("던지기 설정")]
    [HideInInspector] public float throwPower = 3f;
    public float maxAngle = 70f;
    public float minAngle = 30f;
    public float gravity = 9.8f;
    public LayerMask groundWaterMask; // 땅과 물 레이어 마스크

    [Header("찌 궤적 설정")]
    public LineRenderer trajectoryLine;
    public int lineResolution = 30;
    public Transform floatVisual; // 찌 시각적 오브젝트 (필요시)

    private Vector3 throwDirection;
    private bool isThrowing = false;
    private float throwProgress;
    private Vector3[] trajectoryPoints;
    private float totalFlightTime;
    private Vector3 landingPoint;

    public Vector3 Goal => landingPoint;
    public Vector3[] TrajectoryPoints => trajectoryPoints;
    public RaycastHit hit;

    void Awake()
    {
        trajectoryLine.startColor = new Color(1,1,1,0.05f); // 시작 색상
    }

    public void UpdateTray(Transform transform)
    {
        float dragRatio = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
        float currentAngle = Mathf.Lerp(minAngle, maxAngle, dragRatio);
        
        // 던질 방향 계산 (카메라 기준)
        throwDirection = transform.forward;
        throwDirection = Quaternion.AngleAxis(currentAngle, -transform.right) * throwDirection;
        
        // Raycast로 목표 지점 찾기
        CalculateLandingPoint(transform.position + new Vector3(0,.5f,0), throwDirection * throwPower);
        
        // 예시선 업데이트
        UpdateTrajectoryLine();
    }

    private void CalculateLandingPoint(Vector3 startPos, Vector3 initialVelocity)
    {
        // 초기값 설정
        float timeStep = 0.1f;
        float currentTime = 0f;
        Vector3 currentPos = startPos;
        Vector3 lastPos = startPos;
        bool hitFound = false;
        landingPoint = startPos;

        // 궤적 시뮬레이션
        for (int i = 0; i < 100; i++) // 최대 100회 시뮬레이션
        {
            currentTime += timeStep;
            lastPos = currentPos;
            
            // 포물선 위치 계산
            currentPos = startPos + new Vector3(
                initialVelocity.x * currentTime,
                initialVelocity.y * currentTime - 0.5f * gravity * currentTime * currentTime,
                initialVelocity.z * currentTime);
            
            // Raycast로 충돌 체크
            if (Physics.Linecast(lastPos, currentPos, out hit, groundWaterMask))
            {
                landingPoint = hit.point;
                totalFlightTime = currentTime - timeStep + (timeStep * (hit.distance / Vector3.Distance(lastPos, currentPos)));
                hitFound = true;
                break;
            }
        }

        if (!hitFound)
        {
            // 아무것도 맞지 않으면 최대 사정거리 지점 사용
            landingPoint = currentPos;
            totalFlightTime = currentTime;
        }

        // 궤적 포인트 계산
        trajectoryPoints = new Vector3[lineResolution];
        for (int i = 0; i < lineResolution; i++)
        {
            float t = (float)i / (lineResolution - 1) * totalFlightTime;
            trajectoryPoints[i] = startPos + new Vector3(
                initialVelocity.x * t,
                initialVelocity.y * t - 0.5f * gravity * t * t,
                initialVelocity.z * t);
            
            // 착지 지점 이후는 모두 착지 지점으로 고정
            if (i > 0 && trajectoryPoints[i].y <= landingPoint.y)
            {
                trajectoryPoints[i] = landingPoint;
            }
        }
    }

    private void UpdateTrajectoryLine()
    {
        if (trajectoryPoints == null || trajectoryPoints.Length == 0) return;

        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = trajectoryPoints.Length;
        trajectoryLine.SetPositions(trajectoryPoints);

        // 색상 변경 (물에 닿으면 파란색, 땅에 닿으면 빨간색)
        if (Physics.Raycast(landingPoint + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 0.2f, groundWaterMask))
        {
            trajectoryLine.endColor = hit.collider.CompareTag("Water") ? Color.blue : Color.red;
        }
    }
}
