using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SetBindingObj
{

    [MenuItem("Tools/Make Bending Asset %&x")] // Ctrl + Alt + X
    public static void CreateVariantMaterials()
    {
        Material[] baseMaterials = Resources.LoadAll<Material>("Origin Bend");
        if (baseMaterials.Length == 0)
        {
            Debug.LogError("Resources/Materials 폴더에서 불러온 Material이 없습니다!");
            return;
        }

        List<Material> selectedMaterials = new List<Material>();
        foreach (Object obj in Selection.objects)
        {
            if (obj is Material mat)
            {
                selectedMaterials.Add(mat);
            }
        }

        if (selectedMaterials.Count == 0)
        {
            Debug.LogError("선택한 오브젝트 중 Material이 없습니다!");
            return;
        }

        foreach (Material originalMat in selectedMaterials)
        {
            Material newVariant = new Material(baseMaterials[0]);
            newVariant.name = originalMat.name + "_Variant";

            CopyMaterialProperties(originalMat, newVariant);

            string savePath = "Assets/02_Materials/Bending/" + newVariant.name + ".mat";
            AssetDatabase.CreateAsset(newVariant, savePath);
            Debug.Log($"Variant Material 생성됨: {savePath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CopyMaterialProperties(Material source, Material target)
    {
        if (source.HasProperty("_Color"))
            target.SetColor("_Color", source.GetColor("_Color"));

        if (source.HasProperty("_MainTex"))
        {
            Texture tex = source.GetTexture("_MainTex");
            if (tex != null) target.SetTexture("_MainTex", tex);
        }

        if (source.HasProperty("_BumpMap"))
        {
            Texture tex = source.GetTexture("_BumpMap");
            if (tex != null) target.SetTexture("_BumpMap", tex);
        }

        if (source.HasProperty("_OcclusionMap"))
        {
            Texture tex = source.GetTexture("_OcclusionMap");
            if (tex != null) target.SetTexture("_OcclusionMap", tex);
        }

        if (source.HasProperty("_Smoothness"))
            target.SetFloat("_Smoothness", source.GetFloat("_Smoothness"));

        if (source.HasProperty("_Metalic"))
            target.SetFloat("_Metalic", source.GetFloat("_Metalic"));

        if (source.HasProperty("_NormalFloat"))
            target.SetFloat("_NormalFloat", source.GetFloat("_NormalFloat"));
    }
}