using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    public string aiType; // which AI pattern the player uses

    private System.Random random = new System.Random();

    public override IEnumerator TakeTurn()
    {
        // run coroutine based on which strategy the AI is using
        switch(aiType)
        {
            case "random":
                yield return StartCoroutine(RandomAI());
                break;
            default:
                yield return StartCoroutine(PlaceholderAI());
                break;
        }

        yield return null; 
    }

    public override IEnumerator DecideSteal()
    {
        // decide whether to steal based on AI type
        switch(aiType)
        {
            case "random":
                // count number of rubble cards
                int numRubble = 0;
                List<Card> canBattle = new List<Card>();
                foreach(Card card in hand)
                {
                    if(card.rubble)
                    {
                        numRubble++;
                    }
                    else if(!card.rubble && card.atk > 0)
                    {
                        canBattle.Add(card);
                    }
                }

                // 0.5 ^ (1 + num rubble) chance of stealing
                if(random.NextDouble() < Math.Pow(0.5, 1 + numRubble) && canBattle.Count > 0)
                {
                    // pick a random card to send into battle
                    combatant = canBattle[random.Next(0, canBattle.Count)];
                }
                break;
            default:
                combatant = null;
                break;
        }
        yield return null;
    }

    // just discards the last card drawn
    public IEnumerator PlaceholderAI()
    {
        yield return new WaitForSeconds(1.0f);
        Draw();
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(Discard(5));
        
        yield return StartCoroutine(EndTurn());
    }

    // discards random cards
    public IEnumerator RandomAI()
    {
        yield return new WaitForSeconds(1.0f);
        Draw();
        yield return new WaitForSeconds(1.0f);

        List<Card> nonRubble = new List<Card>();
        Card discard;

        // go thru cards that were in hand before drawing
        for(int i = 0; i < 6; i++)
        {
            // add to list if not rubble
            if(!hand[i].rubble)
            {
                nonRubble.Add(hand[i]);
            }
        }

        // if only drawn card was non rubble, discard the drawn card
        if(nonRubble.Count < 2)
        {
            discard = hand[5];
        }
        // discard a random card
        else
        {
            discard = nonRubble[random.Next(0, nonRubble.Count)];
        }

        yield return StartCoroutine(Discard(discard));
        yield return StartCoroutine(EndTurn());
    }

    public override void ArrangeHand()
    {
        Transform layout = transform.Find("Layout" + playerIndex);
        Debug.Log(layout);
        if (layout == null) return;
        if (hand.Count == 5 || hand.Count == 6)
        {
            try
            {
                fiveCardLayout = layout.Find(hand.Count + "CardLayout").gameObject;
            }
            catch (Exception e)
            {
                return;
            }
            for (int i = 0; i < hand.Count; i++)
            {
                Transform card = fiveCardLayout.transform.GetChild(i);
                hand[i].transform.localPosition = card.localPosition;
                hand[i].transform.localRotation = card.localRotation;
                hand[i].transform.localScale = card.localScale;
            }
        }
    }
}
