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
    AutoMoveState _chaseState = AutoMoveState.None;
    Action _chaseEndAction;
    Transform _targetPos;
    Transform _forward;

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

        switch (_chaseState)
        {
            case AutoMoveState.Move:
                _player.playerAnim.SetDirection(Vector3.Lerp(_player.playerAnim.GetDirection(), Vector3.forward, Time.deltaTime * 10f));
                Vector3 target = _targetPos.position - transform.position;
                target.y = 0f;
                Quaternion forward = Quaternion.LookRotation(target);
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, forward, _rotateSpeed * Time.deltaTime * 2f);

                _player.Rigidbody.linearVelocity = target.normalized * _moveSpeed;
                if(target.sqrMagnitude < 0.3f)
                {
                    _chaseState = AutoMoveState.None;
                    _player.playerAnim.SetDirection(Vector3.zero);
                    _chaseEndAction?.Invoke();
                }
                break;
            case AutoMoveState.Rotate:

                Vector3 forw = _forward.forward;
                forw.y = 0f;
                transform.forward = Vector3.MoveTowards(transform.forward, forw, _rotateSpeed * Time.deltaTime * 0.06f);
                print(Vector3.Dot(transform.forward, forw));
                if (Vector3.Dot(transform.forward, forw) > 0.99f)
                {
                    _chaseState = AutoMoveState.None;
                    _player.playerAnim.SetDirection(Vector3.zero);
                    _chaseEndAction?.Invoke();
                }
                break;
        }
    }
    private void TriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (_triggerCnt == 0)
        {
            grounded = true;
            _player.playerAnim.SetBool("Ground", true);
        }
        _triggerCnt++;
    }
    private void TriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        if (!grounded)
        {
            grounded = true;
            _player.playerAnim.SetBool("Ground", true);
        }
    }
    private void TriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        _triggerCnt--;
        if (_triggerCnt == 0)
        {
            grounded = false;
            _player.playerAnim.SetBool("Ground", false);
        }
    }

    void Aim(Vector2 pos)
    {
        if (_chaseState != AutoMoveState.None || _player.boating) return;

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
            grounded = false;
            _player.playerAnim.SetBool("Ground", false);
        }
    }

    public void Move(Vector2 input)
    {
        if (_chaseState != AutoMoveState.None || _player.boating) return;

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
        if (_chaseState != AutoMoveState.None || _player.boating) return;

        if (grounded && _rb.isKinematic == false)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpPower, _rb.linearVelocity.z);
        }
    }
    public void StopMoveTarget()
    {
        _chaseState = AutoMoveState.None;
        _player.playerAnim.SetDirection(Vector3.zero);
        _targetPos = null;
        _chaseEndAction = null;
        _forward = null;
    }
    public void MoveTarget(Transform position, Action endAction)
    {
        _chaseState = AutoMoveState.Move;
        _targetPos = position;
        _chaseEndAction = endAction;
    }
    public void LookTarget(Transform forward, Action endAction)
    {
        _chaseState = AutoMoveState.Rotate;
        _targetPos = null;
        _forward = forward;
        _chaseEndAction = endAction;
    }
}
