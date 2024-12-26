using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum AutoMoveState
{
    None,
    Move,
    Rotate
}
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _camSpeed = 5f;
    [SerializeField] private float _jumpPower = 200f;
    [SerializeField] private float _rotateSpeed = 200f;

    [SerializeField] private Transform _camTrm;
    private Transform _visual;

    public LayerMask groundLayer;
    public bool grounded = true;

    private Rigidbody _rb;
    private Player _player;

    float _pitch = 0f;
    float _yaw = 0f;

    int _triggerCnt = 0;
    public Vector3 direction = Vector3.zero;

    //Target
    AutoMoveState _chaseTarget = AutoMoveState.None;
    Action _chaseEndAction;
    Vector3 _targetPos;
    Quaternion _forward;

    #region UNITY_EVENTS
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        _visual = transform.GetChild(0);
    }

    private void Start()
    {
        _yaw = transform.localEulerAngles.y;
    }

    private void OnEnable()
    {
        _player.playerInput.OnAim += Aim;
        _player.playerInput.DownJump += Jump;
        _player.TriggerEnter += TriggerEnter;
        _player.TriggerStay += TriggerStay;
        _player.TriggerExit += TriggerExit;
    }
    private void OnDisable()
    {
        _player.playerInput.OnAim -= Aim;
        _player.playerInput.DownJump -= Jump;
        _player.TriggerEnter -= TriggerEnter;
        _player.TriggerStay -= TriggerStay;
        _player.TriggerExit -= TriggerExit;
    }
    #endregion

    private void Update()
    {
        FallingCheck();
        switch (_chaseTarget)
        {
            case AutoMoveState.Move:
                Vector3 target = _targetPos - transform.position;
                target.y = 0f;
                _forward = Quaternion.LookRotation(target);
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _forward, _rotateSpeed * Time.deltaTime);
                _player.Rigidbody.linearVelocity = transform.forward * _moveSpeed;
                break;
            case AutoMoveState.Rotate:
                break;
        }
    }
    private void TriggerEnter(Collider other)
    {
        if (_triggerCnt == 0)
        {
            _player.playerAnim.anim.SetInteger("Landing", 1);
        }
        _triggerCnt++;
    }
    private void TriggerStay(Collider other)
    {
        if (!grounded)
        {
            grounded = true;
        }
    }
    private void TriggerExit(Collider other)
    {
        _triggerCnt--;
        if (_triggerCnt == 0)
        {
            _player.playerAnim.anim.SetInteger("Landing", 0);
            _player.playerAnim.SetTrigger("Falling");
            grounded = false;
        }
    }

    void Aim(Vector2 pos)
    {
        if (Cursor.visible == false)
        {
            _yaw += _camSpeed * 0.1f * pos.x;
            transform.localEulerAngles = new Vector3(0, _yaw, 0);

            _pitch += _camSpeed * 0.1f * pos.y;
            _pitch = Mathf.Clamp(_pitch, -60f, 80f);

            _camTrm.localEulerAngles = new Vector3(-_pitch, 0f, 0f);
        }
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

    public void Move(Vector2 input)
    {
        int weight = 1;

        if (_player.playerInput.Shift) weight *= 2;

        Vector3 localMovement = new Vector3(input.x * weight, 0f, input.y * weight);

        if (grounded)
            direction = Vector3.Lerp(direction, localMovement, 7f * Time.deltaTime);
        _player.playerAnim.SetDirection(direction);


        Vector3 velocity = _rb.linearVelocity;


        if (input.sqrMagnitude < 0.1f) return;

/*        Quaternion targetRotation = Quaternion.LookRotation(localMovement);
        _visual.localRotation = Quaternion.RotateTowards(_visual.localRotation, targetRotation, _rotateSpeed * Time.deltaTime);*/
        /*if (!aiming)
        {
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0f, 90f, 0f);
            _visual.localRotation = Quaternion.RotateTowards(_visual.localRotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }*/

        Vector3 worldMovement = transform.TransformDirection(direction);

        velocity.x = worldMovement.x * _moveSpeed;
        velocity.z = worldMovement.z * _moveSpeed;

        velocity.y = _rb.linearVelocity.y;

        if (velocity.sqrMagnitude > _rb.linearVelocity.sqrMagnitude)
            _rb.linearVelocity = velocity;
    }

    public void Jump()
    {
        if (grounded && _rb.isKinematic == false)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpPower, _rb.linearVelocity.z);
        }
    }
    public void StopMoveTarget()
    {
        _chaseTarget = AutoMoveState.None;
        _targetPos = transform.position;
        _chaseEndAction = null;
        _forward = Quaternion.identity;
    }
    public void MoveTarget(Vector3 position, Action endAction)
    {
        _chaseTarget = AutoMoveState.Move;
        _targetPos = position;
        _chaseEndAction = endAction;
    }
    public void LookTarget(Vector3 forward)
    {
        _chaseTarget = AutoMoveState.Rotate;
        _targetPos = transform.position;
        _forward = Quaternion.LookRotation(forward);
    }
}
