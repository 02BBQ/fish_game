using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Steamworks;
using UnityEngine.SocialPlatforms.Impl;

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
    public Transform visual;

    public LayerMask groundLayer;
    public bool grounded = true;
    public bool movable = true;

    private Rigidbody _rb;
    private Player _player;

    float _pitch = 0f;
    float _yaw = 0f;
    int triggerCnt = 0;

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
        visual = transform.GetChild(0);
    }

    private void Start()
    {
        _yaw = transform.localEulerAngles.y;
        if (SteamManager.Initialized)
        {
            bool bSuccess = SteamUserStats.RequestCurrentStats();
            print(bSuccess);
        }
    }

    private void OnEnable()
    {
        _player.playerInput.OnAim += Aim;
        _player.playerInput.DownJump += Jump;
        _player.playerTrigger.TriggerEnter.AddListener(TriggerEnter);
        _player.playerTrigger.TriggerStay.AddListener(TriggerStay);
        _player.playerTrigger.TriggerExit.AddListener(TriggerExit);
    }
    private void OnDisable()
    {
        _player.playerInput.OnAim -= Aim;
        _player.playerInput.DownJump -= Jump;
        _player.playerTrigger.TriggerEnter.RemoveListener(TriggerEnter);
        _player.playerTrigger.TriggerStay.RemoveListener(TriggerStay);
        _player.playerTrigger.TriggerExit.RemoveListener(TriggerExit);
    }
    private void Update()
    {
        if (!movable) return;
        switch (_chaseState)
        {
            case AutoMoveState.Move:
                _player.playerAnim.SetDirection(Vector3.Lerp(_player.playerAnim.GetDirection(), Vector3.forward, Time.deltaTime * 10f));
                Vector3 target = _targetPos.position - transform.position;
                target.y = 0f;
                Quaternion forward = Quaternion.LookRotation(-target);
                visual.localRotation = Quaternion.RotateTowards(visual.localRotation, forward, _rotateSpeed * Time.deltaTime * 2f);

                if (Physics.Raycast(new Ray(transform.position + Vector3.up * 0.1f, target), 0.4f))
                {
                    if (grounded && _rb.isKinematic == false)
                    {
                        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpPower, _rb.linearVelocity.z);
                    }
                }

                _rb.linearVelocity = new Vector3(target.normalized.x * _moveSpeed, _rb.linearVelocity.y, target.normalized.z * _moveSpeed);

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
                visual.forward = Vector3.MoveTowards(visual.forward, forw, _rotateSpeed * Time.deltaTime * 0.02f);
                if (Vector3.Dot(visual.forward, forw) > 0.99f)
                {
                    _chaseState = AutoMoveState.None;
                    _player.playerAnim.SetDirection(Vector3.zero);
                    _chaseEndAction?.Invoke();
                }
                break;
        }
    }
    #endregion

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
    private void TriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        grounded = true;
        triggerCnt++;
        _player.playerAnim.SetBool("Ground", true);
    }
    private void TriggerStay(Collider other)
    {
        if (other.isTrigger) return;
        grounded = true;
        _player.playerAnim.SetBool("Ground", true);
    }
    private void TriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        triggerCnt--;
        if (triggerCnt > 0)
            return;

        grounded = false;
        if(!_player.boating)
        _player.playerAnim.SetBool("Ground", false);
    }


    public void Move(Vector2 input)
    {
        if (_chaseState != AutoMoveState.None || _player.boating) return;
        if (!movable) return;

        int weight = 1;

        if (_player.playerInput.Shift) weight *= 2;

        Vector3 localMovement = input.x * Camera.main.transform.right + input.y * Camera.main.transform.forward;
        localMovement.y = 0f;
        localMovement.Normalize();
        localMovement *= weight;
        direction = Vector3.Lerp(direction, localMovement, 8f * Time.deltaTime);

        Vector3 worldMovement = transform.TransformDirection(direction);
/*        if(Physics.Raycast(new Ray(transform.position + worldMovement * 2f + Vector3.up * 3f, Vector3.down), out RaycastHit hit, 8f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                _player.playerAnim.SetFloat("Y", Mathf.Lerp(_player.playerAnim.anim.GetFloat("Y"), 0f, Time.deltaTime * 8f));
                return;
            }
        }*/
        _player.playerAnim.SetFloat("Y", direction.magnitude);

        Vector3 velocity = _rb.linearVelocity;

        if (input.sqrMagnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        visual.rotation = Quaternion.RotateTowards(visual.rotation, targetRotation, _rotateSpeed * Time.deltaTime * 2f);

        velocity.x = localMovement.x * _moveSpeed;
        velocity.z = localMovement.z * _moveSpeed;

        velocity.y = _rb.linearVelocity.y;

        if (velocity.sqrMagnitude > _rb.linearVelocity.sqrMagnitude)
            _rb.linearVelocity = velocity;
    }

    public void Jump()
    {
        if (_chaseState != AutoMoveState.None || _player.boating) return;
        if (!movable) return;

        if (grounded && _rb.isKinematic == false)
        {
            grounded = false;
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpPower, _rb.linearVelocity.z);

            if (SteamManager.Initialized)
            {
                if (SteamUserStats.GetStat("Test", out int currentStatValue))
                {
                    Debug.Log($"Current Stat Value: {currentStatValue}");

                    int newStatValue = currentStatValue + 1;
                    SteamUserStats.SetStat("Test", newStatValue);

                    if (newStatValue >= 10)
                    {
                        if (SteamUserStats.GetAchievement("TestAchivement", out bool isAchieved))
                        {
                            if (!isAchieved)
                            {
                                // Achievement 달성
                                SteamUserStats.SetAchievement("TestAchivement");
                                SteamUserStats.StoreStats();
                                Debug.Log("Achievement Unlocked!");
                            }
                        }

                        // 4. 변경사항 저장
                        SteamUserStats.StoreStats();
                        Debug.Log($"Stat updated to: {newStatValue}");
                    }
                }
            }
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
