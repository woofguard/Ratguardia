﻿using System.Collections;
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
    [HideInInspector] public bool isStealing;
    [HideInInspector] public int playerIndex; // which index this player is in the Board array
    [HideInInspector] public Card combatant;  // card fighting in battle, used for stealing

    public GameObject fiveCardLayout;
    public GameObject sixCardLayout;

    // function containing player actions on their turn
    public abstract IEnumerator TakeTurn();

    public virtual IEnumerator EndTurn()
    {
        // if not in battle
        yield return new WaitWhile(() => Board.main.refBoardUI.stealUI.activeSelf);
        Board.main.GiveTurn(Board.main.NextPlayer());
        yield return null;
    }

    public Card Draw()
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
        ArrangeHand();
        
        score = CalculateScore();

        return newCard;
    }

    // discard using a specific card reference
    public IEnumerator Discard(Card c)
    {
        Board.main.rubblePile.Push(c);
        c.transform.SetParent(Board.main.rubblePile.transform);
        Board.main.OrganizeRubble();
        hand.Remove(c);

        // set card's owner to -1 (no player)
        c.owner = -1;
        c.rubble = true;
        
        c.ResetStats();
        
        ArrangeHand();
        score = CalculateScore();
        yield return StartCoroutine(PromptSteal());
    }

    // discard using the cards index in the hand
    public IEnumerator Discard(int i)
    {
        Card c = hand[i];
        yield return StartCoroutine(Discard(c));
    }

    // steal a card from the rubble pile
    public void Steal()
    {
        Card stolen = Board.main.rubblePile.Pop();
        stolen.owner = playerIndex;
        hand.Add(stolen);
        stolen.transform.SetParent(transform);
        ArrangeHand();
        score = CalculateScore();
    }

    // ask other players if they want to steal when you discard
    public IEnumerator PromptSteal()
    {
        List<Card> combatants = new List<Card>();
        
        // ask each player if they want to steal
        foreach(var player in Board.main.players)
        {
            // if not the player who discarded and player can steal
            if(player.playerIndex != this.playerIndex && player.CanSteal())
            {
                // if player decides to steal, add card to combatants list
                yield return StartCoroutine(player.DecideSteal());
                if(player.combatant != null)
                {
                    combatants.Add(player.combatant);
                }
            }
        }

        // if anyone wants to steal, run the battle
        if(combatants.Count > 0)
        {
            yield return StartCoroutine(Board.main.Battle(combatants));
            Card winner = Board.main.winner;
            Board.main.players[winner.owner].Steal();

            // if the winner used a cavalier, activate its effect
            if(winner.cardName == "Cavalier")
            {
                winner.ActivateEffect();
            }

            yield return StartCoroutine(Board.main.players[winner.owner].Discard(winner));
            Board.main.winner = null;
        }
    }

    // choose whether to steal
    public abstract IEnumerator DecideSteal();

    // checks conditions for stealing
    public bool CanSteal()
    {
        foreach(Card c in hand)
        {
            // player has a card that isn't rubble and has atk points
            if(c.atk > 0 && !c.rubble)
            {
                return true;
            }
        }

        return false;
    }

    // look thru player's hand and add up all the def scores
    public int CalculateScore()
    {
        score = 0; // reset before recalculating

        List<Card> kings = new List<Card>();

        foreach(Card card in hand)
        {
            // activate effects based on card name
            switch(card.cardName)
            {    
                case "Queen":
                    card.ActivateEffect();
                    break;
                case "Peasant":
                    card.ActivateEffect();
                    break;
                case "King":
                    kings.Add(card);
                    break;
                case "Knight":
                    card.ActivateEffect();
                    break;
            }
            score += card.def;
        }

        // activate king effects after initial score is added up
        foreach(Card king in kings)
        {
            king.ActivateEffect();
        }

        return score;
    }

    // flips entire hand face up at final score calculation
    public void FlipHand()
    {
        foreach(var card in hand)
        {
            card.Flip(true);
        }
    }

    public virtual void ArrangeHand()
    {
        Debug.Log("Arranging player " + playerIndex + "'s hand");
        if (fiveCardLayout == null || sixCardLayout == null) return;
        if (hand.Count == 5)
        {
            for (int i = 0; i < 5; i++)
            {
                Transform card = fiveCardLayout.transform.GetChild(i);
                hand[i].transform.localPosition = card.localPosition;
                hand[i].transform.localRotation = card.localRotation;
                hand[i].transform.localScale = card.localScale;
            }
        }
        else if (hand.Count == 6)
        {
            for (int i = 0; i < 6; i++)
            {
                Transform card = sixCardLayout.transform.GetChild(i);
                hand[i].transform.position = card.position;
                hand[i].transform.rotation = card.rotation;
                hand[i].transform.localScale = card.localScale;
            }
        }
    }
}
