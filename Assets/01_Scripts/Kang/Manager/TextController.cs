using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public List<TextMeshProUGUI> texts;
    private static List<TextMeshProUGUI> _texts;

    private void Awake()
    {
        _texts = texts;
    }
    public static void ApplyText(string TextName, string format, params object[] n)
    {
        for(int i = 0; i < _texts.Count; i++)
        {
            if(_texts[i].gameObject.name == TextName)
            {
                _texts[i].text = string.Format(format, n);
            }
        }
    }
}
