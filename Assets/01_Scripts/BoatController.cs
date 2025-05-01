using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoatController : MapEntity
{
    public BoatDataSO boatData;
    [SerializeField] ParticleSystem[] forms;
    [SerializeField] Transform fishContainer;
    private Rigidbody rigid;
    public Transform camPos;
    BoatEdge[] boatEdges;

    [HideInInspector] public Transform ridePoint;
    AudioSource aud;
    bool fast = false;

    [HideInInspector] public List<FishSO> fishs = new List<FishSO>();
    public FishModel fishPref;
    public Transform fishTank;
    List<FishModel> pool;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
        ridePoint = transform.Find("RidePoint");

        boatEdges = GetComponentsInChildren<BoatEdge>();
        pool = new List<FishModel>();
        isMove = true;
    }
    private void OnEnable()
    {
        InventoryManager.Instance.OnAddFish += AddFish;
        InventoryManager.Instance.OnRemoveFish += RemoveFish;
    }
    private void OnDisable()
    {
        InventoryManager.Instance.OnAddFish -= AddFish;
        InventoryManager.Instance.OnRemoveFish -= RemoveFish;
    }
    protected override void Start()
    {
        base.Start();

        rigid.angularDamping = boatData.boatDamp;
        rigid.linearDamping = boatData.boatDamp;
        rigid.mass = 10000;
    }
    protected override void Update()
    {
        base.Update();
        if (!fast && rigid.linearVelocity.sqrMagnitude > 70f)
        {
            fast = true;
            aud.DOFade(1f, 0.8f);
            foreach (ParticleSystem particle in forms)
                particle.Play();
        }
        else if (fast && rigid.linearVelocity.sqrMagnitude <= 70f)
        {
            fast = false;
            aud.DOFade(0f, 0.8f);
            foreach (ParticleSystem particle in forms)
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void OnDestroy()
    {
        fast = false;
        aud.volume = 0f;
        foreach (ParticleSystem particle in forms)
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public void Move(Vector2 input)
    {
        rigid.mass = boatData.boatMass;
        rigid.AddTorque(transform.up * (input.x * Time.deltaTime * boatData.boatSpeed), ForceMode.Force);
        rigid.AddForce(ridePoint.forward * (input.y * Time.deltaTime * boatData.boatSpeed), ForceMode.Force);
    }

    public bool CanExitBoat()
    {
        foreach(BoatEdge edges in boatEdges)
        {
            if (edges.CheckEdge(0.5f))
            {
                return true;
            }
        }
        return false;
    }

    internal void ExitBoat()
    {
        rigid.mass = 10000;
        IconEnable();
    }


    public void ResetPos(Transform trm)
    {
        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(trm.position, trm.rotation);
        StartCoroutine(StopForce(trm));
    }
    IEnumerator StopForce(Transform trm)
    {
        for (int i = 0; i < 20; i++)
        {
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(trm.position, trm.rotation);
            yield return null;
            yield return null;
            yield return null;
        }
    }
    public void AddFish(List<FishSO> lst, FishSO fish)
    {
        if (lst != fishs) return;

        fishs.Add(fish);

        // pool에서 비활성화된 오브젝트 찾기
        FishModel fishObj = pool.FirstOrDefault(obj => !obj.gameObject.activeSelf);

        if (fishObj == null)
        {
            // 새 오브젝트 생성
            fishObj = Instantiate(fishPref, fishTank);
            pool.Add(fishObj);
        }
        fishObj.Init(fish.image, fish);
        fishObj.gameObject.SetActive(true);
    }

    public void RemoveFish(List<FishSO> lst, FishSO fish)
    {
        if (lst != fishs) return;

        if (fishs.Contains(fish))
        {
            fishs.Remove(fish);
            // 해당 물고기 오브젝트 찾아서 비활성화
            FishModel fishObj = pool.FirstOrDefault(obj => obj.gameObject.activeSelf && obj.fishSO.nameStr == fish.nameStr);
            if (fishObj != null)
            {
                fishObj.gameObject.SetActive(false);
            }
        }
    }
    internal void EnterBoat()
    {
        IconDisable();
    }
}