using UnityEngine;

public class GlobalShaderReplacement : MonoBehaviour
{
    public Shader vertexShader; // Vertex ������ ���̴�
    public Camera cam;

    void OnEnable()
    {
        cam.RenderWithShader(vertexShader, "RenderType");
    }

    void OnDisable()
    {
        if (cam != null)
        {
            cam.ResetReplacementShader(); // ���� ���̴��� �ǵ���
        }
    }
}
