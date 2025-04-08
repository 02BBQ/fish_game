using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuideText : SingleTon<GuideText>
{
    TextMeshProUGUI text;
    HashSet<string> guides;
    public List<SerializeDic> coments;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        guides = new HashSet<string>();
    }
    private void Start()
    {
        Refresh();
    }
    public void AddGuide(string key)
    {
        guides.Add(key);
        Refresh();
    }
    public void RemoveGuide(string key)
    {
        guides.Remove(key);
        Refresh();
    }
    private void Refresh()
    {
        text.text = "";
        foreach(string s in guides)
        {
            text.text += FindValue(s) + "\n";
        }
    }
    private string FindValue(string key)
    {
        foreach (SerializeDic dic in coments)
        {
            if (dic.key == key)
                return dic.value;
        }
        return "";
    }
}
[Serializable]
public struct SerializeDic
{
    public string key;
    public string value;
}
