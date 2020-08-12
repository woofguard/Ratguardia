using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : Player
{
    public override IEnumerator TakeTurn()
    {
        yield return null;
    } 

    public override IEnumerator DecideSteal()
    {
        yield return null;
    }
}
