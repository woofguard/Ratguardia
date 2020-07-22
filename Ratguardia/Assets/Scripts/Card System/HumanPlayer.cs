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
        // for now just if player clicks the mouse, change later
        if(cursor.confirmPressed)
        {
            cursor.confirmPressed = false;
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
        // again just player clicks mouse for now, change later
        if(cursor.confirmPressed)
        {
            cursor.confirmPressed = false;
            Card discarded = Discard(0);
            return true;
        }
        else
        {
            return false;
        }
    }
}
