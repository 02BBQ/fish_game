using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerBoat : MonoBehaviour
{
    public CinemachineCamera boatCam;

    private Player _player;
    BoatController _currentBoat;

    bool _ridable = false;

    #region UNITY_EVENTS
    private void Awake()
    {
        _player = GetComponent<Player>();
    }
    private void OnEnable()
    {
        _player.TriggerEnter += TriggerEnter;
        _player.TriggerExit += TriggerExit;
        _player.playerInput.ClickInteract += TryInterect;
    }

    private void OnDisable()
    {
        _player.TriggerEnter -= TriggerEnter;
        _player.TriggerExit -= TriggerExit;
        _player.playerInput.ClickInteract -= TryInterect;
    }

    private void Update()
    {
        if (_player.boating)
        {
            Vector3 targetPos = _currentBoat.ridePoint.position;
            targetPos.y = Mathf.Max(transform.position.y, targetPos.y);
            transform.position = targetPos;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _currentBoat.ridePoint.eulerAngles.y, transform.eulerAngles.z);
        }   
    }
    #endregion

    private void TryInterect()
    {
        if (!_ridable) return;

        if (_player.boating)
        {
            if (!_currentBoat.CanExitBoat()) return;

            _player.boating = false;
            _player.playerMovement.StopMoveTarget();
            boatCam.Follow = null;
            boatCam.Priority = -1;
        }
        else
        {
            _player.playerMovement.MoveTarget(_currentBoat.ridePoint, () =>
            _player.playerMovement.LookTarget(_currentBoat.ridePoint, () => {
                _player.boating = true;
                boatCam.Priority = 10;
            }));
            boatCam.transform.SetPositionAndRotation(_currentBoat.transform.position, _currentBoat.transform.rotation);
            boatCam.Follow = _currentBoat.camPos;
            
        }

    }

    private void TriggerEnter(Collider other)
    {
        if(other.transform.root.TryGetComponent(out _currentBoat))
        {
            _ridable = true;
        }
    }
    private void TriggerExit(Collider other)
    {
        if(_currentBoat && other.transform.root == _currentBoat.transform)
            _ridable = false;
    }
    public void Move(Vector2 input)
    {
        if(_currentBoat)
            _currentBoat.Move(input);
    }
}
