using UnityEngine;

public class StaticTarget : MonoBehaviour
{
    public static Transform target;
    private void Awake()
    {
        target = transform;
    }
}
