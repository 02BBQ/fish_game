using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    Vector3 _direction = Vector3.zero;
    private void Awake()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    public void SetDirection(Vector3 vector)
    {
        _direction = vector;
        anim.SetFloat("X", _direction.x);
        anim.SetFloat("Y", _direction.z);
    }
    public void SetTrigger(string name)
    {
        anim.SetTrigger(name);
    }
}
