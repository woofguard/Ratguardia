using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    public override IEnumerator TakeTurn()
    {
        StartCoroutine(PlaceholderAI());
        yield return null; 
    }

    public override IEnumerator EndTurn()
    {
        StartCoroutine(base.EndTurn());
        yield return null;
    }

    // just discards the last card drawn
    public IEnumerator PlaceholderAI()
    {
        Draw();
        yield return new WaitForSeconds(0.75f);
        Discard(5);
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(EndTurn());
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
