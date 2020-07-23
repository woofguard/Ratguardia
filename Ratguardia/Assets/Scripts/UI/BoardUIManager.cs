using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardUIManager : MonoBehaviour
{
    public TextMeshProUGUI deckCount;

    public void UpdateDeckUI()
    {
        deckCount.text = Board.main.deck.stack.Count + "";
    }
}
