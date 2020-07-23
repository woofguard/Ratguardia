using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffects : MonoBehaviour
{
    // activated during score calculation
    public void PeasantEffect(Card card)
    {
        // find owner in board
        // find number of peasants
        // if num peasants == 4
        // def = 10
    }

    // activated when a card is added to hand
    public void KnightEffect(Card card)
    {
        // find owner in board
        // find number of rubble in hand
        // atk = num rubble
    }

    // activated after battle
    public void CavalierEffect(Card card)
    {
        
        // find owner in board
        // get last card in hand
        // def +1
    }

    // activated during score calculation
    public void KingEffect(Card card)
    {
        // find owner in board

        // double their score
    }

    // activated when a card is added to/removed from hand
    public void QueenEffect(Card card)
    {
        // go to owner in board
        // count number of queens in player's hand
        // go to card
        // def = num queens
    }

    // activated in final game score calculation
    public void WitchEffect(Card card)
    {
        // for each player on board
            // if player is not owner
            // score -2
    }

    // activated in final game score calculation
    public void JesterEffect(Card card)
    {
        // for each player on board
            // for each king they have
            // half their score
    }
}
