using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] private Rigidbody _rigid;
    public Rigidbody Rigidbody { get => _rigid; }
    [HideInInspector] public PlayerAnimation playerAnim;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerBoat playerBoat;
    [HideInInspector] public bool boating = false;
    public PlayerInput playerInput;

    CapsuleCollider _capsuleCollider;
    [SerializeField] List<Collider> bodyCollider;

    public Action<Collision> CollisionEnter;
    public Action<Collider> TriggerEnter;
    public Action<Collider> TriggerStay;
    public Action<Collider> TriggerExit;


    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();

        playerAnim = GetComponent<PlayerAnimation>();
        playerMovement = GetComponent<PlayerMovement>();
        playerBoat = GetComponent<PlayerBoat>();
        _capsuleCollider = transform.Find("Collider").GetComponent<CapsuleCollider>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (boating)
            playerBoat.Move(playerInput.Movement);
        else
            playerMovement.Move(playerInput.Movement);
    }
    public void StartPhysics()
    {
        _capsuleCollider.enabled = false;
        OnBodyCollider();
        _rigid.isKinematic = true;
    }
    public void EndPhysics()
    {
        _capsuleCollider.enabled = true;
        _rigid.isKinematic = false;
        OffBodyCollider();
    }

    public void OffBodyCollider()
    {
        foreach(Collider collider in bodyCollider)
        {
            collider.enabled = false;
        }
    }
    public void OnBodyCollider()
    {
        foreach (Collider collider in bodyCollider)
        {
            collider.enabled = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        CollisionEnter?.Invoke(collision);
    }
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
