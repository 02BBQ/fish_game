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

    public event Action ClickESC;
    public event Action InventoryEvent;

    private void OnEnable()
    {
        _inputAction = new InputSystem_Actions();
        _inputAction.UI.Enable();
        _inputAction.UI.Esc.performed += Esc_performed; 
        _inputAction.UI.Inventory.performed += (obj) => InventoryManager.Instance.inventoryUI.SetActive(!InventoryManager.Instance.inventoryUI.activeSelf);
    }

    private void Esc_performed(InputAction.CallbackContext obj)
    {
        ClickESC?.Invoke();
    }

    private void OnDisable()
    {
        _inputAction.UI.Disable();
    }
}
