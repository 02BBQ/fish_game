using System;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] BoatDataSO _boatData;
    private Rigidbody rigid;

    [HideInInspector] public Transform ridePoint;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        ridePoint = transform.Find("RidePoint");
    }
    private void Start()
    {
        rigid.mass = _boatData.boatMass;
        rigid.angularDamping = _boatData.boatDamp;
        rigid.linearDamping = _boatData.boatDamp;
    }

    public void Move(Vector2 input)
    {
        rigid.AddForce(transform.forward * (input.x * Time.deltaTime * _boatData.boatSpeed), ForceMode.Force);

    }
}
