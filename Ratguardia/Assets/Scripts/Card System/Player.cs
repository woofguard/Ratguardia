using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<Card> hand;
    public bool faceUp; // whether the hand is face up or not (true for human player)
    public bool revealed;
    public CardStack rubblePile;

    // public Character character;
    public int score;
    public bool hasTurn;
    public int playerIndex; // which index this player is in the Board array

    // function containing player actions on their turn
    public virtual IEnumerator TakeTurn()
    {
        Draw();
        yield return null;
    }

    public virtual IEnumerator EndTurn()
    {
        Board.main.GiveTurn(Board.main.NextPlayer());
        yield return null;
    }

    public void Draw()
    {
        var newCard = Board.main.deck.Pop();

        // flip card to the same side as rest of hand
        newCard.Flip(faceUp);
        newCard.Reveal(revealed);

        hand.Add(newCard);

        // Debug.Log("Player " + playerIndex + " drew a card");
    }

    // discard using a specific card reference
    public void Discard(Card c)
    {
        rubblePile.Push(c);
        hand.Remove(c);
        PromptSteal();
    }

    // discard using the cards index in the hand
    public void Discard(int i)
    {
        rubblePile.Push(hand[i]);
        hand.RemoveAt(i);
        PromptSteal();
    }

    // steal someone else's card from their rubble pile
    public void Steal(Card c)
    {

    }

    // ask other players if they want to steal when you discard
    public void PromptSteal()
    {

    }

    public int CalculateScore()
    {
        return 0;
    }
}
