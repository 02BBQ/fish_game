using DG.Tweening;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class BoatController : MapEntity
{
    [SerializeField] BoatDataSO _boatData;
    [SerializeField] ParticleSystem[] forms;
    private Rigidbody rigid;
    public Transform camPos;
    BoatEdge[] boatEdges;

    [HideInInspector] public Transform ridePoint;
    AudioSource aud;
    bool fast = false;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
        ridePoint = transform.Find("RidePoint");

        boatEdges = GetComponentsInChildren<BoatEdge>();
        isMove = true;
    }
    protected override void Start()
    {
        base.Start();

        rigid.angularDamping = _boatData.boatDamp;
        rigid.linearDamping = _boatData.boatDamp;
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


    public void ResetPos(Transform trm)
    {
        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(trm.position, trm.rotation);
        StartCoroutine(StopForce(trm));
    }
    IEnumerator StopForce(Transform trm)
    {
        for (int i = 0; i < 30; i++)
        {
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(trm.position, trm.rotation);
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
        }
    }

    internal void EnterBoat()
    {
        IconDisable();
    }
}