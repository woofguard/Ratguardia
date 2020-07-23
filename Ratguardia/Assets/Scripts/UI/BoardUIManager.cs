using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardUIManager : MonoBehaviour
{
    public TextMeshProUGUI deckCount;

    // Currently updates all UI every frame
    // should be fixed from Board class to only update relevant UI on events
    void Update()
    {
        UpdateDeckUI();
    }

    public void UpdateDeckUI()
    {
        deckCount.text = Board.main.deck.stack.Count + "";
    }
}
