using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardUIManager : MonoBehaviour
{
    public TextMeshProUGUI deckCount; // UI displaying # of cards left in the deck

    // prompts to draw and discard cards
    public TextMeshProUGUI drawPrompt;
    public TextMeshProUGUI discardPrompt;

    // steal-related UI objects
    public GameObject stealPrompt;
    public GameObject stealUI;
    public bool stealChosen;
    public bool stealing;

    // inspection-related UI objects
    public GameObject inspectUI;

    // placeholder prompts to display player scores and winner
    public TextMeshProUGUI p0Score;
    public TextMeshProUGUI p1Score;
    public TextMeshProUGUI p2Score;
    public TextMeshProUGUI p3Score;
    public TextMeshProUGUI winner;

    public GameObject restartPrompt;

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

    public void PromptSteal(bool display)
    {
        stealPrompt.transform.Find("Prompt").gameObject.SetActive(display);
    }

    public void DecideSteal(bool decision)
    {
        stealChosen = true;
        stealing = decision;
        PromptSteal(false);
    }

    public void ResetSteal()
    {
        stealChosen = stealing = false;
        PromptSteal(false);
        PromptChooseSteal(false);
    }

    public void PromptChooseSteal(bool display)
    {
        stealPrompt.transform.Find("Card Choose Prompt").gameObject.SetActive(display);
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

    public IEnumerator InspectCard(Card card)
    {
        HumanPlayer hp = (HumanPlayer)Board.main.players[0];
        
        // only inspect during player's turn
        if(hp.hasTurn || hp.isStealing)
        {
            hp.cursor.cancelPressed = false;

            inspectUI.SetActive(true);
            UICard display = inspectUI.GetComponentInChildren<UICard>();

            display.InspectCard(card);

            yield return new WaitUntil(() => hp.cursor.cancelReleased);

            hp.cursor.cancelReleased = false;
            inspectUI.SetActive(false);
        }
        else
        {
            hp.cursor.cancelPressed = false;

            yield return new WaitUntil(() => hp.cursor.cancelReleased);
            hp.cursor.cancelReleased = false;
        }
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
