using System;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] BoatDataSO _boatData;
    private Rigidbody rigid;
    //Vector2 currentVelocity;

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
        rigid.AddTorque(transform.up * (input.x * Time.deltaTime * _boatData.boatSpeed), ForceMode.Force);
        rigid.AddForce(transform.forward * (input.y * Time.deltaTime * _boatData.boatSpeed), ForceMode.Force);
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