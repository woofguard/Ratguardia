using System.Collections;
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
}
