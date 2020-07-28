using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public Cursor cursor;

    public override IEnumerator TakeTurn()
    {
        Board.main.refBoardUI.PromptDraw();
        yield return new WaitUntil(() => PlayerDraws());
        
        Board.main.refBoardUI.PromptDiscard();
        yield return new WaitUntil(() => PlayerDiscards());
        yield return StartCoroutine(Discard(cursor.clickedCard));
        cursor.clickedCard = null;
        
        Board.main.refBoardUI.HidePrompts();
        yield return StartCoroutine(EndTurn());
    }

    public override IEnumerator DecideSteal()
    {
        Debug.Log("Steal? left click: yes | right click: no");
        isStealing = true;
        yield return new WaitUntil(() => cursor.confirmPressed || cursor.cancelPressed);

        if(cursor.confirmPressed)
        {
            cursor.confirmPressed = false;

            Debug.Log("Click a card you want to send to battle");
            yield return new WaitUntil(() => PlayerDecidesCombatant());
            isStealing = false;
        }
        else if(cursor.cancelPressed)
        {
            cursor.cancelPressed = false;
            isStealing = false;
        }
    }

    // returns true when the player draws a card
    private bool PlayerDraws()
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

    private bool PlayerDiscards()
    {
        Card card = cursor.clickedCard;

        // if the player clicks their own card, card is not rubble
        if(cursor.confirmPressed && card != null && card.owner == playerIndex && !card.rubble)
        {
            cursor.confirmPressed = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool PlayerDecidesCombatant()
    {
        Card card = cursor.clickedCard;

        // player clicks a non rubble card with atk points
        if(cursor.confirmPressed && card != null && card.owner == playerIndex && !card.rubble && card.atk > 0)
        {
            cursor.confirmPressed = false;

            // store combatant card
            cursor.clickedCard = null;
            combatant = card;
            return true;
        }
        else
        {
            return false;
        }
    }
}
