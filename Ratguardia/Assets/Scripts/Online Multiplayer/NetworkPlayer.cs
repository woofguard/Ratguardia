using System;
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
}
