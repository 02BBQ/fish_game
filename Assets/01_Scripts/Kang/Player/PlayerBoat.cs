using DG.Tweening;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerBoat : MonoBehaviour
{
    public CinemachineCamera boatCam;

    private Player _player;
    BoatController _currentBoat;
    [SerializeField] Color _ridingColor;
    [SerializeField] Color _originColor;

    bool _ridable = false;

    #region UNITY_EVENTS
    private void Awake()
    {
        _player = GetComponent<Player>();
    }
    private void OnEnable()
    {
        _player.playerTrigger.TriggerEnter.AddListener(TriggerEnter);
        _player.playerTrigger.TriggerExit.AddListener(TriggerExit);
        EventBus.Subscribe(EventBusType.Drowning, Reset);
        _player.AddInteract(TryInterect);
        _player.playerInput.OnClickC += OpenFish;
    }

    private void OnDisable()
    {
        _player.playerTrigger.TriggerEnter.RemoveListener(TriggerEnter);
        _player.playerTrigger.TriggerExit.RemoveListener(TriggerExit);
        EventBus.Unsubscribe(EventBusType.Drowning, Reset);
        _player.RemoveInterect(TryInterect);
        _player.playerInput.OnClickC -= OpenFish;
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

    private void OpenFish()
    {
        UIManager.Instance.fishTank.SetActive(true);
        InventoryManager.Instance.SetFish(_currentBoat.boatData.maxFish, _currentBoat.fishs);
    }
    private void TryInterect()
    {

        if (_player.boating)
        {
            GuideText.Instance.RemoveGuide("ExitBoat");
            GuideText.Instance.AddGuide("EnterBoat");
            _player.boating = false;
            _player.playerMovement.StopMoveTarget();
            _currentBoat.ExitBoat();
            _player.sr.color = _originColor;
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
                    GuideText.Instance.AddGuide("ExitBoat");
                    GuideText.Instance.RemoveGuide("EnterBoat");
                    Transform visual = _player.playerMovement.visual;
                    visual.localEulerAngles = new Vector3(visual.localEulerAngles.x, 0f, visual.localEulerAngles.z);
                    _player.boating = true;
                    boatCam.Priority = 10;
                    _currentBoat.EnterBoat();
                    _player.sr.color = _ridingColor;
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
            GuideText.Instance.AddGuide("EnterBoat");
            GuideText.Instance.AddGuide("SmallWaterTank");
            _ridable = true;
        }
    }
    private void TriggerExit(Collider other)
    {
        if (_currentBoat && other.transform.root == _currentBoat.transform)
        {
            GuideText.Instance.RemoveGuide("EnterBoat");
            GuideText.Instance.RemoveGuide("SmallWaterTank");
            UIManager.Instance.fishTank.SetActive(false);
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
        _player.sr.color = _originColor;
        boatCam.Follow = null;
        boatCam.Priority = -1;
        _currentBoat = null;
    }
}
