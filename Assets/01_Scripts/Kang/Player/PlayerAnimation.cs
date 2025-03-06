using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    public event Action OnCastRod;
    Vector3 _direction = Vector3.zero;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetDirection(Vector3 vector)
    {
        _direction = vector;
        anim.SetFloat("X", _direction.x);
        anim.SetFloat("Y", _direction.z);
    }
    public void SetTrigger(string name) => anim.SetTrigger(name);
    public void ResetTrigger(string name) => anim.ResetTrigger(name);
    public void SetBool(string name, bool value) => anim.SetBool(name, value);
    public void SetFloat(string name, float value) => anim.SetFloat(name, value);
    public void SetInt(string name, int value) => anim.SetInteger(name, value);

    public Vector3 GetDirection()
    {
        return new Vector3(anim.GetFloat("X"), 0, anim.GetFloat("Y"));
    }

    public void CastRod()
    {
        OnCastRod?.Invoke();
    }
}
