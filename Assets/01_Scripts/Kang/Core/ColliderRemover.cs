/*using UnityEngine;
using UnityEditor;

public static class ColliderRemover
{
    // Ctrl + Shift + C를 누르면 실행됨
    //[MenuItem("Tools/Remove Colliders %#c")]
    public static void RemoveSelectedColliders()
    {
        GameObject[] selectedObjects = Selection.gameObjects; // 현재 선택된 오브젝트 가져오기

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("선택된 오브젝트가 없습니다.");
            return;
        }

        int removedCount = 0;

        foreach (GameObject obj in selectedObjects)
        {
            Collider collider = obj.GetComponent<Collider>(); // Collider 컴포넌트 가져오기
            if (collider != null)
            {
                Object.DestroyImmediate(collider); // 즉시 삭제
                removedCount++;
                Debug.Log($"Collider removed from: {obj.name}");
            }
        }

        Debug.Log($"총 {removedCount}개의 Collider가 제거되었습니다.");
    }

    //[MenuItem("Tools/Remove Colliders %#c", true)]
    private static bool ValidateRemoveSelectedColliders()
    {
        return Selection.gameObjects.Length > 0;
    }
}*/