using UnityEngine;

public class Dock : Interactor
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        GuideText.Instance.AddGuide("Market");
    }
    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        GuideText.Instance.RemoveGuide("Market");
    }
    protected override void OnInterect()
    {
        UIManager.Instance.dock.SetActive(true);
    }

}
