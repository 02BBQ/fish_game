using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTarget : MonoBehaviour
{
    public static Transform target;
    private void Awake()
    {
        target = transform;
    }
}
