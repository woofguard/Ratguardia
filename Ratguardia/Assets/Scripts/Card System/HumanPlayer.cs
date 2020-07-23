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
            Debug.Log("drew a " + drawn);
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
}
