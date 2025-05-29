using System.Collections.Generic;
using UnityEngine;
 
public class Dock : Interactor
{
    public List<Transform> points;
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            base.OnTriggerEnter(other);
            BoatManager.Instance.SetDock(this);
            
            GuideText.Instance.AddGuide("Dock");
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            base.OnTriggerExit(other);
            UIManager.Instance.dock.SetActive(false);
            GuideText.Instance.RemoveGuide("Dock");
        }
    }
    protected override void OnInterect()
    {
        UIManager.Instance.dock.SetActive(true);
    }
}