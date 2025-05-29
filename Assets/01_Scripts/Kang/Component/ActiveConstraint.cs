using UnityEngine;

public class ActiveConstraint : MonoBehaviour
{
    [SerializeField] private GameObject child;
    private void OnEnable()
    {
        child.SetActive(true);
    }
    private void OnDisable()
    {
        if(child != null)
            child.SetActive(false);
    }
}
