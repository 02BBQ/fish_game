using System;
using Unity.Cinemachine;
using UnityEngine;

public class BoatController : MapEntity
{
    [SerializeField] BoatDataSO _boatData;
    private Rigidbody rigid;
    public Transform camPos;
    //Vector2 currentVelocity;
    BoatEdge[] boatEdges;

    [HideInInspector] public Transform ridePoint;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        ridePoint = transform.Find("RidePoint");

        boatEdges = GetComponentsInChildren<BoatEdge>();
    }
    protected override void Start()
    {
        isDynamic = true;
        base.Start();

        rigid.angularDamping = _boatData.boatDamp;
        rigid.linearDamping = _boatData.boatDamp;
        rigid.mass = 10000;
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