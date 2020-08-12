using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class CutsceneCursor : MonoBehaviour
{
  //  public Player player;
    [HideInInspector] public Controls controls;

    [HideInInspector] public bool confirmPressed;
    [HideInInspector] public bool cancelPressed;
    [HideInInspector] public bool cancelReleased;
    [HideInInspector] public Card clickedCard;

    private void Awake()
    {
        controls = new Controls();

        // add callbacks to each input action
        controls.CardGame.Confirm.performed += OnConfirm;
        controls.CardGame.Cancel.started += CancelPressed;  

        controls.CardGame.Confirm.Enable();  
        controls.CardGame.Cancel.Enable();
        controls.CardGame.CursorPosition.Enable();

        confirmPressed = false;
        cancelPressed = false;
        cancelReleased = false;
        clickedCard = null;
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        confirmPressed = true;
    }

    public void CancelPressed(InputAction.CallbackContext context)
    {
      
    }

    public void CancelReleased(InputAction.CallbackContext context)
    {
        
    }
}
