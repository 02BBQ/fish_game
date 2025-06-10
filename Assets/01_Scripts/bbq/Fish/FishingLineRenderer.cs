using System.Collections.Generic;
using UnityEngine;

public class FishingLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int vertexCount = 12;
    [SerializeField] private float catenaryA = 35.0f;

    private void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.useWorldSpace = true;
    }

    public void UpdateLine(Vector3 startPoint, Vector3 endPoint)
    {
        var pointList = CalculateCatenaryPoints(startPoint, endPoint);
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }

    private List<Vector3> CalculateCatenaryPoints(Vector3 startPoint, Vector3 endPoint)
    {
        var pointList = new List<Vector3>();
        float length = Vector3.Distance(startPoint, endPoint);

        for (int i = 0; i <= vertexCount; i++)
        {
            float t = (float)i / vertexCount;
            float x = t * length;
            float sag = -catenaryA * ((float)System.Math.Cosh((x - length / 2) / catenaryA) - (float)System.Math.Cosh(length / (2 * catenaryA)));
            
            Vector3 basePoint = Vector3.Lerp(startPoint, endPoint, t);
            Vector3 offset = Vector3.down * sag;
            
            pointList.Add(basePoint + offset);
        }

        return pointList;
    }
} 