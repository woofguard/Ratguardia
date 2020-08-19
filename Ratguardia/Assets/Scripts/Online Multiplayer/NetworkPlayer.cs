﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : Player
{
    public override IEnumerator TakeTurn()
    {
        // wait for player to draw
        yield return StartCoroutine(NetworkManager.main.WaitForPacket(RMP.Draw));
        Draw();

        // wait for player to discard
        yield return StartCoroutine(NetworkManager.main.WaitForPacket(RMP.Discard));
        byte[] discardPacket = NetworkManager.main.packet;

        // read which card they discarded from the packet
        yield return StartCoroutine(Discard(discardPacket[1]));

        // wait for player to end turn
        yield return StartCoroutine(NetworkManager.main.WaitForPacket(RMP.EndTurn));
        yield return StartCoroutine(EndTurn());
    } 

    public override IEnumerator DecideSteal()
    {
        yield return null;
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
