using System;
using UnityEngine;

public class PlayerBoat : MonoBehaviour
{

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
    }
    #endregion

    private void TryInterect()
    {
        if (_player.boating)
        {
            _player.boating = false;
            _player.playerMovement.StopMoveTarget();
        }
        else
        {
            if (_ridable)
            {
                _player.boating = true;
                _player.playerMovement.MoveTarget(_currentBoat.ridePoint.position, () => _player.playerMovement.LookTarget(_currentBoat.ridePoint.forward));
            }
        }
    }

    private void TriggerEnter(Collider other)
    {
        _ridable = true;
        _currentBoat = other.GetComponent<BoatController>();
    }
    private void TriggerExit(Collider other)
    {
        _ridable = false;
    }
    public void Move(Vector2 input)
    {
        _currentBoat.Move(input);
    }
}
