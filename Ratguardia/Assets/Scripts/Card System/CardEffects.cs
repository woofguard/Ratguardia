using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffects : MonoBehaviour
{
    // activated during score calculation
    public void PeasantEffect(Card card)
    {
        // if peasants == 4, def = 10
        if(CountCards(card.owner, "Peasant") == 4)
        {
            card.def = 10;
        }
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
        // double the owner's score
        Board.main.players[card.owner].score *= 2;
    }

    // activated when a card is added to/removed from hand
    public void QueenEffect(Card card)
    {
        // def = number of queens in hand
        card.def = CountCards(card.owner, "Queen");
    }

    // activated in final game score calculation
    public void WitchEffect(Card card)
    {
        // for each player on board
        for(int i = 0; i < 4; i++)
        {
            // if player is not owner, -2 score
            if(i != card.owner)
            {
                Board.main.scores[i] -= 2;
            }
        }
    }

    // activated in final game score calculation
    public void JesterEffect(Card card)
    {
        // for each player on board
        for(int i = 0; i < 4; i++)
        {
            // count how many kings they have
            int numKings = CountCards(i, "King");

            // half their score for each king they have
            if(numKings > 0)
            {
                Board.main.scores[i] /= (2 * numKings);
            }
        }
    }

    // gets the number of a certain card in a player's hand
    private int CountCards(int playerIndex, string cardName)
    {
        int num = 0;
        foreach(var card in Board.main.players[playerIndex].hand)
        {
            if(card.cardName == cardName)
            {
                num++;
            }
        }

        return num;
    }
}
