using System;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public Action<Collider> TriggerEnter;
    public Action<Collider> TriggerStay;
    public Action<Collider> TriggerExit;
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit?.Invoke(other);
    }
}
