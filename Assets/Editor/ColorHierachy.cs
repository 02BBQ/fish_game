/*using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColorHierachy : MonoBehaviour
{
#if UNITY_EDITOR
    private static Dictionary<object, ColorHierachy> coloredObjects = new Dictionary<object, ColorHierachy>();
    static ColorHierachy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleDraw;
    }

    private static void HandleDraw(int id, Rect selectionRect)
    {
        Object obj = EditorUtility.InstanceIDToObject(id);

        if (obj != null && coloredObjects.ContainsKey(obj))
        {
            GameObject gObj = obj as GameObject;
            ColorHierachy ch = gObj.GetComponent<ColorHierachy>();
            if (ch != null)
            {
                PaintObject(obj, selectionRect, ch);
            }
            else
            {
                coloredObjects.Remove(obj);
            }
        }
    }

    private static void PaintObject(Object obj, Rect selectionRect, ColorHierachy ch)
    {
        Rect bgRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width + 50, selectionRect.height);

        if (Selection.activeObject != obj)
        {
            EditorGUI.DrawRect(bgRect, ch.backColor);
            string name = $"{ch.prefix} {obj.name}";

            EditorGUI.LabelField(bgRect, name, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = ch.fontColor },
                fontStyle = FontStyle.Bold
            });
        }
    }

    public string prefix;
    public Color backColor = Color.black;
    public Color fontColor = Color.black;

    private void Reset()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (coloredObjects.ContainsKey(this.gameObject) == false)
        {
            coloredObjects.Add(this.gameObject, this);
        }
    }

#endif
}*/