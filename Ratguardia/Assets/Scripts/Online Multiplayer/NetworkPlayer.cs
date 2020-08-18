using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : Player
{
    public override IEnumerator TakeTurn()
    {
        // wait for player to draw
        NetworkManager.main.WaitForPacket(RMP.Draw);
        Draw();

        // wait for player to discard
        NetworkManager.main.WaitForPacket(RMP.Discard);
        byte[] discardPacket = NetworkManager.main.packet;

        // read which card they discarded from the packet
        Discard(discardPacket[1]);

        // wait for player to end turn
        NetworkManager.main.WaitForPacket(RMP.EndTurn);
        yield return StartCoroutine(EndTurn());
    } 

    public override IEnumerator DecideSteal()
    {
        yield return null;
    }
}
