﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Cursor : MonoBehaviour
{
    public Player player;
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
        if(player.hasTurn || player.isStealing)
        {
            confirmPressed = true;

            // get position of cursor
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(controls.CardGame.CursorPosition.ReadValue<Vector2>());
            
            // whatever the heck a raycast is
            var clickedObj = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);

            // if we clicked anything
            if(clickedObj.collider != null)
            {
                // get the clicked card and store it for HumanPlayer to read
                clickedCard = clickedObj.collider.gameObject.GetComponent<DisplayCard>().card;
            }
        }     
    }

    public void CancelPressed(InputAction.CallbackContext context)
    {
        // get position of cursor
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(controls.CardGame.CursorPosition.ReadValue<Vector2>());
        
        // whatever the heck a raycast is
        var clickedObj = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);

        // if we clicked anything
        if(clickedObj.collider != null)
        {
            cancelPressed = true;

            // add callback now so it isn't tracking it if empty space is pressed
            controls.CardGame.Cancel.canceled += CancelReleased; 

            // inspect the clicked card
            var clicked = clickedObj.collider.gameObject.GetComponent<DisplayCard>().card;
            StartCoroutine(Board.main.refBoardUI.InspectCard(clicked));
        }
    }

    public void CancelReleased(InputAction.CallbackContext context)
    {
        cancelReleased = true;
        controls.CardGame.Cancel.canceled -= CancelReleased;
    }
}
