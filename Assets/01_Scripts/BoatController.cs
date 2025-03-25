using DG.Tweening;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class BoatController : MapEntity
{
    [SerializeField] BoatDataSO _boatData;
    [SerializeField] ParticleSystem[] forms;
    private Rigidbody rigid;
    public Transform camPos;
    //Vector2 currentVelocity;
    BoatEdge[] boatEdges;

    [HideInInspector] public Transform ridePoint;
    AudioSource aud;
    Vector3 originPos;
    Quaternion originRot;
    bool fast = false;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
        ridePoint = transform.Find("RidePoint");

        boatEdges = GetComponentsInChildren<BoatEdge>();
        transform.GetPositionAndRotation(out originPos, out originRot);
    }
    protected override void Start()
    {
        isDynamic = true;
        base.Start();

        rigid.angularDamping = _boatData.boatDamp;
        rigid.linearDamping = _boatData.boatDamp;
        rigid.mass = 10000;
    }
    private void Update()
    {
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

    private void OnEnable()
    {
        EventBus.Subscribe(EventBusType.Drowning, OnDrowning);
    }

    private void OnDisable()


    {
        EventBus.Unsubscribe(EventBusType.Drowning, OnDrowning);

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
        rigid.mass = _boatData.boatMass;
        rigid.AddTorque(transform.up * (input.x * Time.deltaTime * _boatData.boatSpeed), ForceMode.Force);
        rigid.AddForce(ridePoint.forward * (input.y * Time.deltaTime * _boatData.boatSpeed), ForceMode.Force);
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


    private void OnDrowning()
    {
        rigid.linearVelocity = Vector3.zero;
        transform.SetPositionAndRotation(originPos, originRot);
    }

    internal void EnterBoat()
    {
        IconDisable();
    }
    /*    public void Move(Vector2 input)
   {
       currentVelocity += input * _boatData.boatWeight * Time.deltaTime;
       currentVelocity.x = Mathf.Min(currentVelocity.x, _boatData.boatSpeed);
       currentVelocity.y = Mathf.Min(currentVelocity.y, _boatData.boatSpeed);

   }
   private void LateUpdate()
   {
       rigid.angularVelocity = new Vector3(rigid.angularVelocity.x, currentVelocity.x * 10f, rigid.angularVelocity.z);
       rigid.linearVelocity = transform.TransformDirection(new Vector3(rigid.linearVelocity.x, rigid.linearVelocity.y, currentVelocity.y * 80f));

       currentVelocity -= Vector2.one * _boatData.boatWeight * Time.deltaTime;
       currentVelocity.x = Mathf.Max(currentVelocity.x, 0f);
       currentVelocity.y = Mathf.Max(currentVelocity.y, 0f);
   }*/
}