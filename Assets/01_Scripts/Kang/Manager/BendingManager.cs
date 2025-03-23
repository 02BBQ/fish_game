using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class BendingManager : MonoBehaviour
{
    private const string BENDING_FEATURE = "ENABLE_BENDING";
    private void Awake()
    {
        print("asdf");
        if (Application.isPlaying)
            Shader.EnableKeyword(BENDING_FEATURE);
        else
            Shader.DisableKeyword(BENDING_FEATURE);
    }
    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }
    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }
    private static void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        camera.ResetCullingMatrix();
    }
    private static void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        float size = camera.farClipPlane / 2f;
        camera.cullingMatrix = Matrix4x4.Ortho(-size, size, -size, size, camera.nearClipPlane, camera.farClipPlane) * camera.worldToCameraMatrix;
    }
}
