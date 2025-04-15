using System.Collections.Generic;
using UnityEngine;
 
public class Dock : Interactor
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            base.OnTriggerEnter(other);
            GuideText.Instance.AddGuide("Dock");
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            base.OnTriggerExit(other);
            GuideText.Instance.RemoveGuide("Dock");
        }
    }
    protected override void OnInterect()
    {
        UIManager.Instance.dock.SetActive(true);
    }
}