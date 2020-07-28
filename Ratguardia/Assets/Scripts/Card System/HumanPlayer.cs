using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public Cursor cursor;

    public override IEnumerator TakeTurn()
    {
        // show player's hand in console
        // Debug.Log("Your Hand:");
        // foreach(Card c in hand)
        // {
        //     Debug.Log(c);
        // }

        yield return new WaitUntil(() => PlayerDraws());
        yield return new WaitUntil(() => PlayerDiscards());
        StartCoroutine(EndTurn());
    }

    public override IEnumerator EndTurn()
    {
        StartCoroutine(base.EndTurn());
        yield return null;   
    }

    public override IEnumerator DecideSteal()
    {
        isStealing = true;

        Board.main.refBoardUI.PromptSteal(true);
        yield return new WaitUntil(() => Board.main.refBoardUI.stealChosen);

        if(Board.main.refBoardUI.stealing)
        {
            cursor.confirmPressed = false;

            Board.main.refBoardUI.PromptChooseSteal(true);

            yield return StartCoroutine(DecideCombatant());

            Board.main.refBoardUI.PromptChooseSteal(false);

            isStealing = false;
        }
        else
        {
            cursor.confirmPressed = false;
            isStealing = false;
        }

        Board.main.refBoardUI.ResetSteal();
    }

    // player clicks the card they want to use to battle
    private IEnumerator DecideCombatant()
    {
        yield return new WaitUntil(() => cursor.confirmPressed);

        // player clicked a card in their hand
        if(cursor.confirmPressed && cursor.clickedCard != null && cursor.clickedCard.owner == playerIndex)
        {
            cursor.confirmPressed = false;
            
            // store combatant card
            Card clicked = cursor.clickedCard;
            cursor.clickedCard = null;
            combatant = clicked;
        }
        else
        {
            combatant = null;
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

        // if the player clicks their own card
        if(cursor.confirmPressed && card != null && card.owner == playerIndex)
        {
            cursor.confirmPressed = false;
            StartCoroutine(Discard(card));
            cursor.clickedCard = null;
            return true;
        }
        else
        {
            return false;
        }
    }
}
