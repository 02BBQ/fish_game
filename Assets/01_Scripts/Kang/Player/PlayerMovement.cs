using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _camSpeed = 5f;
    [SerializeField] private float _jumpPower = 200f;

    [SerializeField] private Transform _camTrm;

    public LayerMask groundLayer;
    public bool grounded = true;

    private Rigidbody _rb;
    private Player _player;

    float _pitch = 0f;
    float _yaw = 0f;


    Vector3 _direction = Vector3.zero;
    int _triggerCnt = 0;

    bool _lockY = false;
    bool _lockX = false;

    #region UNITY_EVENTS
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
    }
    private void Start()
    {
        _yaw = transform.localEulerAngles.y;
    }
    private void OnEnable()
    {
        _player.playerInput.OnAim += Aim;
        _player.playerInput.DownJump += Jump;
    }
    private void Update()
    {
        FallingCheck();
        Move(_player.playerInput.Movement);
    }
    private void OnDisable()
    {
        _player.playerInput.OnAim -= Aim;
        _player.playerInput.DownJump -= Jump;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_triggerCnt == 0)
        {
            _player.playerAnim.anim.SetInteger("Landing", 1);
        }
        _triggerCnt++;
    }
    private void OnTriggerStay(Collider other)
    {
        if (!grounded)
        {
            grounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        _triggerCnt--;
        if (_triggerCnt == 0)
        {
            _player.playerAnim.anim.SetInteger("Landing", 0);
            _player.playerAnim.SetTrigger("Falling");
            grounded = false;
        }
    }
    #endregion

    void Aim(Vector2 pos)
    {
        if (!_lockY && Cursor.visible == false)
        {
            _yaw += _camSpeed * 0.1f * pos.x;
            transform.localEulerAngles = new Vector3(0, _yaw, 0);
        }

        if (!_lockX && Cursor.visible == false)
        {
            _pitch += _camSpeed * 0.1f * pos.y;
            _pitch = Mathf.Clamp(_pitch, -60f, 80f);

            _camTrm.localEulerAngles = new Vector3(-_pitch, 1.09f, 3.5f);
        }
    }

    public void LockY()
    {
        _lockY = true;
    }
    public void UnlockY()
    {
        _yaw = transform.localEulerAngles.y;
        _lockY = false;
    }
    public void LockX()
    {
        _lockX = true;
    }
    public void UnlockX()
    {
        _lockX = false;
    }
    public void Lock()
    {
        LockX();
        LockY();
    }
    public void Unlock()
    {
        UnlockX();
        UnlockY();
    }

    private void FallingCheck()
    {
        if (_triggerCnt == 0 && grounded)
        {
            _player.playerAnim.anim.SetInteger("Landing", 0);
            _player.playerAnim.SetTrigger("Falling");
            grounded = false;
        }
    }

    void Move(Vector2 input)
    {
        int weight = 1;

        if (_player.playerInput.Shift) weight *= 2;

        Vector3 velocity = _rb.linearVelocity;

        Vector3 localMovement = new Vector3(input.x * weight, 0, input.y * weight);
        _direction = Vector3.Lerp(_direction, localMovement, 7f * Time.deltaTime);
        _player.playerAnim.SetDirection(_direction);

        Vector3 worldMovement = transform.TransformDirection(_direction);

        velocity.x = worldMovement.x * _moveSpeed;
        velocity.z = worldMovement.z * _moveSpeed;

        _rb.linearVelocity = velocity;
    }

    public void Jump()
    {
        if (grounded && _rb.isKinematic == false)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpPower, _rb.linearVelocity.z);
        }
    }
}
