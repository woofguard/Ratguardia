using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public Cursor cursor;

    public override IEnumerator TakeTurn()
    {
        // show player's hand in console
        Debug.Log("Your Hand:");
        foreach(Card c in hand)
        {
            Debug.Log(c);
        }

        yield return new WaitUntil(() => PlayerDraws());
        yield return new WaitUntil(() => PlayerDiscards());
        StartCoroutine(EndTurn());
    }

    public override IEnumerator EndTurn()
    {
        StartCoroutine(base.EndTurn());
        yield return null;   
    }

    public override Card DecideSteal()
    {
        Debug.Log("Steal? left click: yes | right click: no");
        StartCoroutine(ConfirmSteal());

        if(cursor.confirmPressed)
        {
            cursor.confirmPressed = false;

            Debug.Log("Click a card you want to send to battle");
            return DecideCombatant();
        }
        else if(cursor.cancelPressed)
        {
            cursor.cancelPressed = false;
            return null;
        }

        return null;
    }

    // returns true when the player draws a card
    public bool PlayerDraws()
    {
        Card card = cursor.clickedCard;

        // if the player clicks a card that is unowned and not rubble (i.e. deck)
        if(cursor.confirmPressed && card != null && card.owner < 0 && !card.rubble)
        {
            // reset cursor data
            cursor.confirmPressed = false;
            cursor.clickedCard = null;

            Card drawn = Draw();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerDiscards()
    {
        Card card = cursor.clickedCard;

        // if the player clicks their own card
        if(cursor.confirmPressed && card != null && card.owner == playerIndex)
        {
            cursor.confirmPressed = false;
            Card discarded = Discard(card);
            cursor.clickedCard = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    // player clicks to steal or not
    public IEnumerator ConfirmSteal()
    {
        yield return new WaitUntil(() => cursor.confirmPressed || cursor.cancelPressed);
    }

    // player clicks the card they want to use to battle
    public Card DecideCombatant()
    {
        StartCoroutine(PlayerClicksCombatant());

        if(cursor.confirmPressed && cursor.clickedCard != null && cursor.clickedCard.owner == playerIndex)
        {
            cursor.confirmPressed = false;
            Card clicked = cursor.clickedCard;
            cursor.clickedCard = null;
            return clicked;
        }

        return null;
    }

    public IEnumerator PlayerClicksCombatant()
    {
        yield return new WaitUntil(() => cursor.confirmPressed);
    }
}
