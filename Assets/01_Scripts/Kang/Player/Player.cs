using Steamworks;
using System;
using System.Collections;
using UnityEngine;

public class Player : MapEntity
{
    [HideInInspector] private Rigidbody _rigid;
    public Rigidbody Rigidbody { get => _rigid; }
    public PlayerAnimation playerAnim;
    public PlayerMovement playerMovement;
    public PlayerBoat playerBoat;
    public Trigger playerTrigger;
    public PlayerSlot playerSlot;

    [HideInInspector] public bool boating = false;
    [HideInInspector] public ConstantForce cForce;

    public PlayerInput playerInput;
    public LayerMask mapLayer;

    CapsuleCollider _capsuleCollider;

    public Action<Collision> CollisionEnter;

    public GameObject fishObj;
    SpriteRenderer fishRenderer;
    MeshFilter fishMesh;
    [HideInInspector] public FishSO currentFish = null;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        // playerAnim = GetComponentInChildren<PlayerAnimation>();
        cForce = GetComponent<ConstantForce>();
        _capsuleCollider = transform.Find("Collider").GetComponent<CapsuleCollider>();
        fishRenderer = fishObj.GetComponent<SpriteRenderer>();
        fishMesh = fishObj.GetComponent<MeshFilter>();
    }
    //public Item debugItem;
    protected override void Start()
    {
        if (SteamManager.Instance.useSteam)
        {
            isMove = true;
            isRotate = true;
            playerInput.InputAction.Enable();
            base.Start();
            //InventoryManager.Instance.AddItem(debugItem);
            playerBoat.enabled = true;
            Rigidbody.mass = 10f;
            cForce.enabled = true;
            if (SteamManager.Initialized)
                SteamUserStats.ResetAllStats(true);
        }
    }
    protected override void Update()
    {
        base.Update();
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
        _rigid.isKinematic = true;
    }
    public void EndPhysics()
    {
        _capsuleCollider.enabled = true;
        _rigid.isKinematic = false;
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
        Transform camTarget = CameraManager.Instance.camVirtual.LookAt;
        if (camParent == null)
            yield break;

        playerMovement.movable = false;
        var localPos = CameraManager.Instance.camVirtual.transform.localPosition;
        var localRot = CameraManager.Instance.camVirtual.transform.localRotation;
        CameraManager.Instance.camVirtual.transform.parent = null;
        CameraManager.Instance.camVirtual.Follow = null;

        UIManager.Instance.FadeIn(1.5f);
        yield return new WaitForSeconds(2.5f);

        playerMovement.movable = true;
        transform.position = Definder.GameManager.spawnPoint.position;
        CameraManager.Instance.camVirtual.transform.parent = camParent;
        CameraManager.Instance.camVirtual.Follow = camTarget;
        CameraManager.Instance.camVirtual.transform.SetLocalPositionAndRotation(localPos, localRot);
        EventBus.Publish(EventBusType.Drowning);
        boating = false;
        
        UIManager.Instance.FadeOut(1f);
    }

    public void HandleFish(FishSO fishSO)
    {
        currentFish = fishSO;
        if (fishSO.visualPath != "")
        {
            Definder.GameManager.LoadAddressableAsset(fishSO.visualPath, (obj) =>
            {
                fishRenderer.material = obj.GetComponent<Renderer>().material;
                fishMesh.mesh = obj.GetComponent<MeshFilter>().mesh;
            });
        }
        else if(fishSO.image != null)
        {
            fishRenderer.sprite = fishSO.image;
        }
        fishObj.SetActive(true);
        InventoryManager.Instance.personalFishSlot.SetItem(currentFish);
    }
    public void DisableFish()
    {
        fishObj.SetActive(false);
    }
}
