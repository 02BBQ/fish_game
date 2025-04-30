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

    public PlayerInput playerInput;
    public LayerMask mapLayer;

    CapsuleCollider _capsuleCollider;

    public Action<Collision> CollisionEnter;

    public GameObject fishObj;
    MeshRenderer fishRenderer;
    MeshFilter fishMesh;
    [HideInInspector] public FishSO currentFish = null;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        // playerAnim = GetComponentInChildren<PlayerAnimation>();
        _capsuleCollider = transform.Find("Collider").GetComponent<CapsuleCollider>();
        fishRenderer = fishObj.GetComponent<MeshRenderer>();
        fishMesh = fishObj.GetComponent<MeshFilter>();
    }
    public Item debugItem;
    protected override void Start()
    {
        isMove = true;
        isRotate = true;
        base.Start();
        InventoryManager.Instance.AddItem(debugItem);
        playerBoat.enabled = true;
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

    internal void HandleFish(FishSO fishSO)
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
            Sprite sprite = fishSO.image;
            Texture2D newTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height
            );
            newTexture.SetPixels(newColors);
            newTexture.Apply();

            fishRenderer.material.SetTexture("_MainTex", newTexture);
        }
        fishObj.SetActive(true);
    }
}
