using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public List<Card> hand;
    public bool faceUp; // whether the hand is face up or not (true for human player)
    public bool revealed;

    // public Character character;
    [HideInInspector] public int score;
    [HideInInspector] public bool hasTurn;
    [HideInInspector] public int playerIndex; // which index this player is in the Board array

    // function containing player actions on their turn
    public abstract IEnumerator TakeTurn();

    public virtual IEnumerator EndTurn()
    {
        Board.main.GiveTurn(Board.main.NextPlayer());
        yield return null;
    }

    public Card Draw()
    {
        var newCard = Board.main.deck.Pop();

        // flip card to the same side as rest of hand
        newCard.Flip(faceUp);
        newCard.Reveal(revealed);

        // set card's owner to this player
        newCard.owner = playerIndex;

        hand.Add(newCard);

        // Debug.Log("Player " + playerIndex + " drew a card");

        return newCard;
    }

    // discard using a specific card reference
    public Card Discard(Card c)
    {
        Board.main.rubblePile.Push(c);
        hand.Remove(c);

        // set card's owner to -1 (no player)
        c.owner = -1;
        c.rubble = true;

        c.ResetStats();
        
        Debug.Log("Player " + playerIndex + " discards a " + c);
        
        PromptSteal();
        return c;
    }

    // discard using the cards index in the hand
    public Card Discard(int i)
    {
        Card c = hand[i];
        return Discard(c);
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
