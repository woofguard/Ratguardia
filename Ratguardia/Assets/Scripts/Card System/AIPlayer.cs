using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    public string aiType; // which AI pattern the player uses

    // seconds to wait before various actions
    private float drawWaitTime = 0.75f;
    private float discardWaitTime = 1.0f;
    private float stealWaitTime = 0.25f;


    private System.Random random = new System.Random();

    public override IEnumerator TakeTurn()
    {
        icon.transform.Find("Outline").gameObject.SetActive(true);

        // run coroutine based on which strategy the AI is using
        switch (aiType)
        {
            case "random":
                yield return StartCoroutine(RandomAI());
                break;
            case "basic":
                yield return StartCoroutine(BasicAI());
                break;
            default:
                yield return StartCoroutine(PlaceholderAI());
                break;
        }

        yield return null;
    }

    public override IEnumerator DecideSteal()
    {
        yield return new WaitForSeconds(stealWaitTime);

        // decide whether to steal based on AI type
        switch (aiType)
        {
            case "random":
                combatant = RandomSteal();
                break;
            case "basic":
                combatant = BasicSteal();
                break;
            default:
                combatant = null;
                break;
        }
        yield return null;
    }

    // just discards the last card drawn
    private IEnumerator PlaceholderAI()
    {
        yield return new WaitForSeconds(drawWaitTime);
        Draw();
        yield return new WaitForSeconds(discardWaitTime);
        yield return StartCoroutine(Discard(5));

        yield return StartCoroutine(EndTurn());
    }

    // discards random cards
    private IEnumerator RandomAI()
    {
        yield return new WaitForSeconds(drawWaitTime);
        Card drawn = Draw();
        yield return new WaitForSeconds(discardWaitTime);

        List<Card> nonRubble = new List<Card>();
        Card discard;

        // go thru cards that were in hand
        for (int i = 0; i < 5; i++)
        {
            // add to list if not rubble
            if (!hand[i].rubble)
            {
                nonRubble.Add(hand[i]);
            }
        }

        // if only drawn card was non rubble, discard the drawn card
        if (nonRubble.Count < 1)
        {
            discard = drawn;
        }
        // discard a random card
        else
        {
            discard = nonRubble[random.Next(0, nonRubble.Count)];
        }

        yield return StartCoroutine(Discard(discard));
        yield return StartCoroutine(EndTurn());
    }

    // has a random chance of stealing
    private Card RandomSteal()
    {
        // count number of rubble cards
        int numRubble = 0;
        List<Card> canBattle = new List<Card>();
        foreach (Card card in hand)
        {
            if (card.rubble)
            {
                numRubble++;
            }
            else if (!card.rubble && card.atk > 0)
            {
                canBattle.Add(card);
            }
        }

        // random chance of stealing based on rubble cards
        if (random.NextDouble() < Math.Pow(0.30, 1 + numRubble) && canBattle.Count > 0)
        {
            // pick a random card to send into battle
            return canBattle[random.Next(0, canBattle.Count)];
        }
        else
        {
            return null;
        }
    }

    // tries to build the hand with the highest raw def score, not counting effects
    private IEnumerator BasicAI()
    {
        yield return new WaitForSeconds(drawWaitTime);
        Card drawn = Draw();
        yield return new WaitForSeconds(discardWaitTime);

        List<Card> nonRubble = new List<Card>();
        Card discard = null;

        // go thru cards that were in hand
        for (int i = 0; i < 5; i++)
        {
            // add to list if not rubble
            if (!hand[i].rubble)
            {
                nonRubble.Add(hand[i]);
            }
        }

        // if only drawn card was non rubble, discard the drawn card
        if (nonRubble.Count < 1)
        {
            discard = drawn;
        }
        else
        {
            // go thru non rubble cards
            for (int i = 0; i < nonRubble.Count; i++)
            {
                // if card has less than drawn card, discard it
                if (nonRubble[i].def < drawn.def)
                {
                    discard = nonRubble[i];
                    break;
                }
            }
        }

        // just in case, discard drawn card
        if (discard == null)
        {
            discard = drawn;
        }

        yield return StartCoroutine(Discard(discard));
        yield return StartCoroutine(EndTurn());
    }

    private Card BasicSteal()
    {
        // get cards in hand that can battle
        List<Card> canBattle = new List<Card>();
        foreach (Card card in hand)
        {
            if (!card.rubble && card.atk > 0)
            {
                canBattle.Add(card);
            }
        }

        if (canBattle.Count > 0)
        {
            // steal if discarded card has more def than battle card
            Card discarded = Board.main.rubblePile.Peek();
            foreach (Card card in canBattle)
            {
                if (discarded.def > card.def)
                {
                    return card;
                }
            }

            // if no card has lower def, dont steal
            return null;
        }
        else
        {
            return null;
        }
    }

    public override void ArrangeHand()
    {
        int pIndex = Board.main.GetHumanPlayer().playerIndex;

        int layoutIndex = 1;

        switch(pIndex)
        {
            case 0:
                layoutIndex = playerIndex;
                break;
            case 1:
                layoutIndex = playerIndex - 1;
                if (layoutIndex == -1) layoutIndex = 3;
                break;
            case 2:
                layoutIndex = playerIndex + 2;
                if (layoutIndex == 5) layoutIndex = 1;
                break;
            case 3:
                layoutIndex = playerIndex + 1;
                break;
            default:
                Debug.Log("player index outside the array hewwo???");
                break;
        }

        Transform layout = transform.Find("Layout" + layoutIndex);
        Debug.Log(layout);
        if (layout == null) return;
        if (!(hand.Count == 5 || hand.Count == 6)) return;
        try
        {
            fiveCardLayout = layout.Find(hand.Count + "CardLayout").gameObject;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString()); // puts this here to suppress warning in console
            return;
        }
        for (int i = 0; i < hand.Count; i++)
        {
            Transform card = fiveCardLayout.transform.GetChild(i);
            hand[i].transform.localPosition = card.localPosition;
            hand[i].transform.localRotation = card.localRotation;
            hand[i].transform.localScale = card.localScale;
        }

        Transform iconTrans = fiveCardLayout.transform.Find("Icon");
        icon.transform.localPosition = iconTrans.localPosition;
        icon.transform.localScale = iconTrans.localScale;
    }
}
