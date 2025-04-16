using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "SO/InputUI")]
public class UIInput : ScriptableObject
{
    private InputSystem_Actions _inputAction;
    public InputSystem_Actions InputAction => _inputAction;
    public event Action<Vector2> MouseScroll;
    public Vector2 MousePos => _inputAction.UI.Point.ReadValue<Vector2>();
    public event Action OnClickESC;
    public event Action OnClickMap;
    public event Action OnDownLeft;
    public Action OnLeft;
    public event Action OnUpLeft;
    public event Action InventoryEvent;
    public bool leftClicked = false;
    private void OnEnable()
    {
        _inputAction = new InputSystem_Actions();
        _inputAction.UI.Enable();
        _inputAction.UI.Esc.performed += Esc_performed;
        _inputAction.UI.M.performed += (obj) => OnClickMap?.Invoke();
        _inputAction.UI.Click.performed += Click_performed;
        _inputAction.UI.Click.canceled += Click_canceled;
        _inputAction.UI.ScrollWheel.performed += (obj)=> MouseScroll?.Invoke(obj.ReadValue<Vector2>());
        _inputAction.UI.Inventory.performed += (obj) => InventoryManager.Instance.inventoryUI.SetActive(!InventoryManager.Instance.inventoryUI.activeSelf);
    }
    private void OnDisable()
    {
        _inputAction.UI.Disable();
    }
    private void Click_canceled(InputAction.CallbackContext obj)
    {
        OnDownLeft?.Invoke();
        leftClicked = true;
    }

    private void Click_performed(InputAction.CallbackContext obj)
    {
        OnUpLeft?.Invoke();
        leftClicked = false;
    }

    private void Esc_performed(InputAction.CallbackContext obj)
    {
        OnClickESC?.Invoke();
    }
}
