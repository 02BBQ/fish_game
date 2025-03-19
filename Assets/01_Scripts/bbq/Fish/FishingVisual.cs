using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FishingVisual : MonoBehaviour
{
    public Transform fishingRod;
    public Transform fishingRodTip;
    public Transform bobber;

    private bool isAnchored;
    private Vector3 anchorPosition;

    private Vector3 zero = new Vector3(0, 0, 0);


    [Header("Line Renderer")]
    [field: SerializeField] public LineRenderer linerenderer { get; private set; }
    public int vertexCount = 12;       // 선분의 분할 수
    public float catenaryA = 35.0f;       // 캐터너리 상수 (슬랙 정도)

    private void Awake()
    {
        linerenderer = GetComponent<LineRenderer>();
        linerenderer.useWorldSpace = true;
    }

    private void Update()
    {
        var pointList = new List<Vector3>();

        // 시작점과 끝점
        Vector3 p0 = fishingRodTip.position;
        Vector3 p1 = bobber.position;
        float L = Vector3.Distance(p0, p1);

        // 각 점을 계산합니다.
        for (int i = 0; i <= vertexCount; i++)
        {
            float t = (float)i / vertexCount;
            // 직선상에서의 누적 거리
            float x = t * L;

            // 캐터너리 공식: x=0, L에서는 0, 중앙에서 최대 처짐
            float sag = -catenaryA * ((float)System.Math.Cosh((x - L / 2) / catenaryA) - (float)System.Math.Cosh(L / (2 * catenaryA)));
            
            // 기본 선분 상의 점
            Vector3 basePoint = Vector3.Lerp(p0, p1, t);
            // 중력 방향(아래)으로 sag 만큼 오프셋 적용
            Vector3 offset = Vector3.down * sag;
            
            pointList.Add(basePoint + offset);
        }

        // 라인 렌더러에 점들을 적용
        linerenderer.positionCount = pointList.Count;
        linerenderer.SetPositions(pointList.ToArray());
    }

    public void ResetBobber()
    {
        bobber.localPosition = new Vector3(-0.0241999999f,-0.056499999f,0);
        bobber.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);
    }

    public void SetAnchor(bool isAnchored, Vector3? anchorPosition = null)
    {
        if (anchorPosition == null)
        {
            anchorPosition = zero;
        }
        this.isAnchored = isAnchored;
        this.anchorPosition = (Vector3)anchorPosition;
    }
}
