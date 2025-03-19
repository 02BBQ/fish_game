using UnityEngine;

public class GlobalShaderReplacement : MonoBehaviour
{
    public Shader vertexShader; // Vertex 변형용 쉐이더
    public Camera cam;

    void OnEnable()
    {
        cam.RenderWithShader(vertexShader, "RenderType");
    }

    void OnDisable()
    {
        if (cam != null)
        {
            cam.ResetReplacementShader(); // 원래 쉐이더로 되돌림
        }
    }
}
