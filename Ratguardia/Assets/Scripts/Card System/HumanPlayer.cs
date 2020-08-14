using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public Cursor cursor;

    public override IEnumerator TakeTurn()
    {
        icon.transform.Find("Outline").gameObject.SetActive(true);

        Board.main.refBoardUI.PromptDraw();
        yield return new WaitUntil(() => PlayerDraws());
        
        Board.main.refBoardUI.PromptDiscard();
        yield return new WaitUntil(() => PlayerDiscards());
        Board.main.refBoardUI.HidePrompts();
        yield return StartCoroutine(Discard(cursor.clickedCard));
        cursor.clickedCard = null;
        
        
        yield return StartCoroutine(EndTurn());
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

            Debug.Log("Click a card you want to send to battle");
            yield return new WaitUntil(() => PlayerDecidesCombatant());

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

    public override Card Draw()
    {
        var newCard = Board.main.deck.Pop();
        Board.main.refBoardUI.UpdateDeckUI();

        // flip card to the same side as rest of hand
        newCard.Flip(faceUp);
        newCard.Reveal(revealed);

        // set card's owner to this player
        newCard.owner = playerIndex;


        hand.Add(newCard);
        newCard.transform.SetParent(transform);

        AudioManager.main.sfxDraw.Play();
        ArrangeHand();

        newCard.visualCard.FlipUp();

        score = CalculateScore();

        return newCard;
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
			
			//plays send to battle card animation
			//card.visualCard.sendCard();

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
