using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Cursor : MonoBehaviour
{
    public Controls controls;

    public bool confirmPressed;

    private void Awake()
    {
        controls = new Controls();

        // add callbacks to each input action
        controls.CardGame.Confirm.performed += OnConfirm;
        controls.CardGame.Cancel.performed += OnCancel;
        controls.CardGame.CursorPosition.performed += ReadCursorPos;

        controls.CardGame.Confirm.Enable();  
        controls.CardGame.Cancel.Enable();
        controls.CardGame.CursorPosition.Enable();

        confirmPressed = false;
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        confirmPressed = true;
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        
    }

    public void ReadCursorPos(InputAction.CallbackContext context)
    {
        
    }
}
