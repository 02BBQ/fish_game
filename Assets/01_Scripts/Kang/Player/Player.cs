using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [HideInInspector] private Rigidbody _rigid;
    public Rigidbody Rigidbody { get => _rigid; }
    public PlayerAnimation playerAnim;
    public PlayerMovement playerMovement;
    public PlayerBoat playerBoat;
    public Trigger playerTrigger;

    [HideInInspector] public bool boating = false;

    public PlayerInput playerInput;
    public LayerMask mapLayer;

    CapsuleCollider _capsuleCollider;
    [SerializeField] List<Collider> bodyCollider;

    public Action<Collision> CollisionEnter;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        // playerAnim = GetComponentInChildren<PlayerAnimation>();
        _capsuleCollider = transform.Find("Collider").GetComponent<CapsuleCollider>();
    }
    private void Update()
    {

        if (boating)
            playerBoat.Move(playerInput.Movement);
        else
            playerMovement.Move(playerInput.Movement);

        if(transform.position.y <= -1.75f)
            Die();
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

    public int GetCurrentOcean()
    {
        if (Physics.Raycast(playerMovement.visual.position, Vector3.up, out RaycastHit hit, 1000f, mapLayer))
        {
            return hit.collider.name[hit.collider.name.Length - 1]-48;
        }
        return -1;
    }
    public void AddInteract(Action action)
    {
        playerInput.ClickInteract += action;
    }
    public void RemoveInterect(Action action)
    {
        playerInput.ClickInteract -= action;
    }
    private void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        Transform camParent = CameraManager.Instance.camVirtual.transform.parent;
        if (camParent == null)
            yield break;

        playerMovement.movable = false;
        var localPos = CameraManager.Instance.camVirtual.transform.localPosition;
        var localRot = CameraManager.Instance.camVirtual.transform.localRotation;
        CameraManager.Instance.camVirtual.transform.parent = null;

        UIManager.Instance.FadeIn(1.5f);
        yield return new WaitForSeconds(2.5f);

        playerMovement.movable = true;
        transform.position = Definder.GameManager.spawnPoint.position;
        CameraManager.Instance.camVirtual.transform.parent = camParent;
        CameraManager.Instance.camVirtual.transform.SetLocalPositionAndRotation(localPos, localRot);
        EventBus.Publish(EventBusType.Drowning);
        boating = false;
        
        UIManager.Instance.FadeOut(1f);
    }
}
