using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerBoat : MonoBehaviour
{
    public CinemachineCamera boatCam;

    private Player _player;
    BoatController _currentBoat;
    [SerializeField] Color _ridingColor;
    Color _originColor;

    bool _ridable = false;

    #region UNITY_EVENTS
    private void Awake()
    {
        _player = GetComponent<Player>();
        _originColor = UIManager.Instance.playerIcon.color;
    }
    private void OnEnable()
    {
        _player.playerTrigger.TriggerEnter += TriggerEnter;
        _player.playerTrigger.TriggerExit += TriggerExit;
        EventBus.Subscribe(EventBusType.Drowning, Reset);
        _player.AddInteract(TryInterect);
    }


    private void OnDisable()
    {
        _player.playerTrigger.TriggerEnter += TriggerEnter;
        _player.playerTrigger.TriggerExit += TriggerExit;
        EventBus.Unsubscribe(EventBusType.Drowning, Reset);
        _player.RemoveInterect(TryInterect);
    }

    private void Update()
    {
        if (_player.boating && _currentBoat)
        {
            Vector3 targetPos = _currentBoat.ridePoint.position;
            targetPos.y = Mathf.Max(transform.position.y, targetPos.y);
            transform.position = targetPos;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _currentBoat.ridePoint.eulerAngles.y, transform.eulerAngles.z);
        }
        else if (_player.boating)
        {
            //Debug.Log(_currentBoat);
        }
    }
    #endregion

    private void TryInterect()
    {

        if (_player.boating)
        {
            _player.boating = false;
            _player.playerMovement.StopMoveTarget();
            _currentBoat.ExitBoat();
            UIManager.Instance.playerIcon.color = _originColor;
            boatCam.Follow = null;
            boatCam.Priority = -1;
            _player.playerMovement.visual.localRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            if (!_ridable) return;

            _player.playerMovement.MoveTarget(_currentBoat.ridePoint, () =>
            {
                _player.playerMovement.LookTarget(_currentBoat.ridePoint, () =>
                {
                    Transform visual = _player.playerMovement.visual;
                    visual.localEulerAngles = new Vector3(visual.localEulerAngles.x, 0f, visual.localEulerAngles.z);
                    _player.boating = true;
                    boatCam.Priority = 10;
                    _currentBoat.EnterBoat();
                    UIManager.Instance.playerIcon.color = _ridingColor;
                });
                boatCam.transform.SetPositionAndRotation(_currentBoat.transform.position, _currentBoat.transform.rotation);
                boatCam.Follow = _currentBoat.camPos;
            });
        }
    }

    private void TriggerEnter(Collider other)
    {
        var boat = other.transform.root.GetComponent<BoatController>();
        if (boat != null)
        {
            _currentBoat = boat;
            _ridable = true;
        }
    }
    private void TriggerExit(Collider other)
    {
        if (_currentBoat && other.transform.root == _currentBoat.transform)
        {
            _ridable = false;
        }
    }
    public void Move(Vector2 input)
    {
        if(_currentBoat)
            _currentBoat.Move(input);
    }
    private void Reset()
    {
        _ridable = false;
        if(_currentBoat)
            _currentBoat.ExitBoat();
        _player.playerMovement.StopMoveTarget();
        UIManager.Instance.playerIcon.color = _originColor;
        boatCam.Follow = null;
        boatCam.Priority = -1;
        _currentBoat = null;
    }
}
