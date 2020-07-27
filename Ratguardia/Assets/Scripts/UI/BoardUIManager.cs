using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardUIManager : MonoBehaviour
{
    public TextMeshProUGUI deckCount;

    public TextMeshProUGUI drawPrompt;
    public TextMeshProUGUI discardPrompt;

    public TextMeshProUGUI p0Score;
    public TextMeshProUGUI p1Score;
    public TextMeshProUGUI p2Score;
    public TextMeshProUGUI p3Score;

    public TextMeshProUGUI winner;

    public GameObject stealUI;

    private void Start()
    {
        p0Score.enabled = false;
        p1Score.enabled = false;
        p2Score.enabled = false;
        p3Score.enabled = false;
        winner.enabled = false;

        HidePrompts();
    }

    public void UpdateDeckUI()
    {
        deckCount.text = Board.main.deck.stack.Count + "";
    }

    public void PromptDraw()
    {
        drawPrompt.enabled = true;
        discardPrompt.enabled = false;
    }

    public void PromptDiscard()
    {
        drawPrompt.enabled = false;
        discardPrompt.enabled = true;
    }

    public void HidePrompts()
    {
        drawPrompt.enabled = false;
        discardPrompt.enabled = false;
    }

    public IEnumerator DisplayBattle(List<Card> combatants, Card winner)
    {
        Debug.Log("Battle beginning");
        stealUI.SetActive(true);

        GameObject layout = stealUI.transform.Find(combatants.Count + "CardLayout").gameObject;

        layout.SetActive(true);

        UICard[] cards = layout.GetComponentsInChildren<UICard>();

        for (int i = 0; i < cards.Length; i++)
        {
            if (combatants.Count <= i) break;
            cards[i].DisplayCard(combatants[i]);
            cards[i].SetDim(combatants[i] != winner);
        }

        HumanPlayer hp = (HumanPlayer)Board.main.players[0];
        Debug.Log(hp);

        hp.isStealing = true;
        yield return new WaitUntil(() => hp.cursor.confirmPressed);
        hp.cursor.confirmPressed = false;
        hp.isStealing = false;

        foreach (UICard card in cards)
        {
            card.SetDim(false);
        }
        layout.SetActive(false);
        stealUI.SetActive(false);
    }

    public void DisplayScores()
    {
        p0Score.enabled = true;
        p1Score.enabled = true;
        p2Score.enabled = true;
        p3Score.enabled = true;
        winner.enabled = true;

        p0Score.text = "Final score: " + Board.main.scores[0] + " points";
        p1Score.text = "Final score: " + Board.main.scores[1] + " points";
        p2Score.text = "Final score: " + Board.main.scores[2] + " points";
        p3Score.text = "Final score: " + Board.main.scores[3] + " points";

        winner.text = "Player " + Board.main.DetermineWinner() + " wins!";
    }
}
