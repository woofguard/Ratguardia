using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Cursor : MonoBehaviour
{
    [HideInInspector] public Controls controls;

    [HideInInspector] public bool confirmPressed;
    [HideInInspector] public Card clickedCard;

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
        clickedCard = null;
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        confirmPressed = true;

        // get position of cursor
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(controls.CardGame.CursorPosition.ReadValue<Vector2>());
        
        // whatever the heck a raycast is
        var clickedObj = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);

        // see if we clicked anything
        if(clickedObj.collider != null)
        {
            // get the clicked card and store it for HumanPlayer to read
            clickedCard = clickedObj.collider.gameObject.GetComponent<DisplayCard>().card;
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        
    }

    public void ReadCursorPos(InputAction.CallbackContext context)
    {
        
    }
}
