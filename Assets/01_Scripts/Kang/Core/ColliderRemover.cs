/*using UnityEngine;
using UnityEditor;

public static class ColliderRemover
{
    // Ctrl + Shift + C�� ������ �����
    //[MenuItem("Tools/Remove Colliders %#c")]
    public static void RemoveSelectedColliders()
    {
        GameObject[] selectedObjects = Selection.gameObjects; // ���� ���õ� ������Ʈ ��������

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("���õ� ������Ʈ�� �����ϴ�.");
            return;
        }

        int removedCount = 0;

        foreach (GameObject obj in selectedObjects)
        {
            Collider collider = obj.GetComponent<Collider>(); // Collider ������Ʈ ��������
            if (collider != null)
            {
                Object.DestroyImmediate(collider); // ��� ����
                removedCount++;
                Debug.Log($"Collider removed from: {obj.name}");
            }
        }

        Debug.Log($"�� {removedCount}���� Collider�� ���ŵǾ����ϴ�.");
    }

    //[MenuItem("Tools/Remove Colliders %#c", true)]
    private static bool ValidateRemoveSelectedColliders()
    {
        return Selection.gameObjects.Length > 0;
    }
}*/