using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "SO/InputTest")]
public class PlayerInput : ScriptableObject
{
    private InputSystem_Actions _inputAction;
    public InputSystem_Actions InputAction => _inputAction;

    public event Action<Vector2> OnAim;
    public event Action DownJump;
    public event Action UpJump;
    public event Action ClickEsc;
    public event Action ClickInteract;
    public event Action DownCtrl;
    public event Action UpCtrl;
    public event Action DownShift;
    public event Action UpShift;
    public event Action LCDown;
    public event Action LCUp;
    public Vector2 Movement { get; private set; }
    public bool Shift { get; private set; }
    public bool Jumping { get; private set; }

    private void OnEnable()
    {
        _inputAction = new InputSystem_Actions();
        _inputAction.Player.Enable();
        _inputAction.Player.Look.performed += Aim_performed;
        _inputAction.Player.Move.performed += Movement_performed;
        _inputAction.Player.Move.canceled += Movement_performed;
        _inputAction.Player.Jump.performed += Jump_performed;
        _inputAction.Player.Jump.canceled += Jump_canceled;
        _inputAction.Player.Sprint.performed += Shift_performed;
        _inputAction.Player.Sprint.canceled += Shift_canceled;
        _inputAction.Player.Attack.performed += (obj) => LCDown?.Invoke();
        _inputAction.Player.Attack.canceled += (obj) => LCUp?.Invoke();

        _inputAction.Player.Interact.performed += (obj) => ClickInteract?.Invoke();
        _inputAction.Player.Ctrl.performed += (obj) => DownCtrl?.Invoke();
        _inputAction.Player.Ctrl.canceled += (obj) => UpCtrl?.Invoke();
    }

    private void Shift_canceled(InputAction.CallbackContext obj)
    {
        UpShift?.Invoke();
        Shift = false;
    }

    private void Shift_performed(InputAction.CallbackContext obj)
    {
        DownShift?.Invoke();
        Shift = true;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        Jumping = false;
        UpJump?.Invoke();
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        Movement = obj.ReadValue<Vector2>();
    }
    private void Aim_performed(InputAction.CallbackContext obj)
    {
        OnAim?.Invoke(obj.ReadValue<Vector2>());
    }
    private void Jump_performed(InputAction.CallbackContext obj)
    {
        Jumping = true;
        DownJump?.Invoke();
    }
    private void OnDisable()
    {
        _inputAction.Player.Disable();
    }
}
