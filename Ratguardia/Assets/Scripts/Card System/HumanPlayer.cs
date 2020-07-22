using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public override IEnumerator TakeTurn()
    {
        StartCoroutine(base.TakeTurn());
        StartCoroutine(EndTurn());
        yield return null;
    }

    public override IEnumerator EndTurn()
    {
        StartCoroutine(base.EndTurn());
        yield return null;        
    }
}
