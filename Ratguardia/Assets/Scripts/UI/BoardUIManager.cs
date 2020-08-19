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

    public BeginRoundDisplay brd;
    public ResultsDisplay rd;
    public MatchResultsDisplay mrd;
    public SpareFeedDisplay sfd;
    public GameObject gameOver;


    public GameObject continuePrompt;
    public GameObject restartPrompt;
    public GameObject sfdPrompt;

    private void Start()
    {
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

    public IEnumerator DisplayBeginning()
    {
        brd.DisplayNewRound();
        yield return new WaitUntil(() => !brd.gameObject.activeSelf);
    }

    public IEnumerator DisplayBattle(List<Card> combatants, Card winner)
    {
        Debug.Log("Battle beginning");
		yield return new WaitForSeconds(0.5f);
        stealUI.SetActive(true);

        GameObject layout = stealUI.transform.Find(combatants.Count + "CardLayout").gameObject;

		
		
        layout.SetActive(true);

        UICard[] cards = layout.GetComponentsInChildren<UICard>();

        for (int i = 0; i < cards.Length; i++)
        {
            if (combatants.Count <= i) break;
            cards[i].DisplayCard(combatants[i]);
            cards[i].SetDim(combatants[i] != winner);
			if(combatants[i] == winner)
			{ 
			   GameObject halo = layout.transform.Find("BattleVictorHalo" + i).gameObject;
			   halo.SetActive(true);
		    }				
			
        }

        HumanPlayer hp = Board.main.GetHumanPlayer();
        Debug.Log(hp);

        hp.isStealing = true;
        yield return new WaitUntil(() => hp.cursor.confirmPressed);
        hp.cursor.confirmPressed = false;
        hp.isStealing = false;

        foreach (UICard card in cards)
        {
            card.SetDim(false);
        }
		for(int i = 0; i < cards.Length; i++)
		{
			GameObject halo = layout.transform.Find("BattleVictorHalo" + i).gameObject;
			halo.SetActive(false);
		}
        layout.SetActive(false);
        stealUI.SetActive(false);
    }

    public IEnumerator InspectCard(Card card)
    {
        HumanPlayer hp = Board.main.GetHumanPlayer();
        
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
        rd.gameObject.SetActive(true);
        continuePrompt.SetActive(true);
        rd.DisplayResults();
    }

    public void SetContinue(bool cont)
    {
        Button continueButton = continuePrompt.transform.Find("Restart").GetComponent<Button>();
        continueButton.onClick.RemoveAllListeners();

        if (cont)
        {
            continuePrompt.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Continue to next round?";
            continueButton.onClick.AddListener(StateManager.main.RestartCardGame);
        } else
        {
            continuePrompt.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "View results";
            continueButton.onClick.AddListener(DisplayMatchResults);
        }
    }

    public void DisplayMatchResults()
    {
        rd.gameObject.SetActive(false);
        continuePrompt.SetActive(false);

        mrd.gameObject.SetActive(true);
        restartPrompt.gameObject.SetActive(true);
        mrd.DisplayMatchResults();
    }

    public void DisplayGameOver()
    {
        sfd.gameObject.SetActive(false);
        sfdPrompt.gameObject.SetActive(false);
        gameOver.SetActive(true);
    }
}
