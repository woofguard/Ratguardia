using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public static StateManager main;

    public int[] matchScores;

    public static int match;

    public string[] combatants;
    public Stack<string> replacements;

    public int round;
    public int roundsPerMatch = 3;

    public static string currentCutscene;

    public bool inCutscene = false;

    public int charDeath = -1;

    private void Awake()
    {
        // persistent singleton
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this.gameObject);
            match = 0;
            round = 1;
            combatants = new string[] { "The Jester", "The Peasant", "The Knight", "The Cavalier" };

            replacements = new Stack<string>();
            replacements.Push("The Preyrider");
            replacements.Push("The Assassin");

            matchScores = new int[] { 0, 0, 0, 0 };
            currentCutscene = "Intro";
            Debug.Log(SceneManager.GetActiveScene().name);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadCutscene(string scene)
    {
        Debug.Log("Loading " + currentCutscene + " because the match is " + match);
        inCutscene = true;
        //currentCutscene = scene;
        SceneManager.LoadScene("Cutscene");
    }

    public void LoadTutorial()
    { 
        LoadCutscene("Intro");
    }

    public void AdvanceNarrative()
    {
        Debug.Log("Advancing narrative..." + inCutscene + " " + match);
        if(inCutscene)
        {
            switch(match)
            {
                case 0:
                    combatants = new string[] { "The Child", "The Mother", "The Sibling", "The Father" };
                    break;
                case 1:
                    combatants = new string[] { "The Jester", "The Peasant", "The Knight", "The Cavalier" };
                    break;
                case 2:
                case 3:
                    if (charDeath > -1) ReplaceCombatant(charDeath);
                    charDeath = -1;
                    break;
                case 4:
                    ReplaceCombatant(2, "The King");
                    break;
                case 5:
                    ExitGame();
                    break;
                default:
                    break;
            }
            inCutscene = false;
            ResetMatch();
        }
        else
        {
            inCutscene = true;
            currentCutscene = "Intro";
            switch (match)
            {
                case 0: // i don't think this should happen
                    break;
                case 1:
                    currentCutscene = "Pre Match 1";
                    break;
                case 2:
                case 3:
                    currentCutscene = "Placeholder";
                    break;
                case 4:
                    currentCutscene = "Placeholder";
                    break;
                case 5:
                    currentCutscene = "Placeholder";
                    break;
                default:
                    break;
            }
            LoadCutscene(currentCutscene);
        }
    }

    public void ReplaceCombatant(int combatant, string replacement = "")
    {
        if(replacement != "")
        {
            combatants[combatant] = replacement;
        }
        else
        {
            combatants[combatant] = replacements.Pop();
        }
    }
    
    public void StartMultiplayer()
    {
        

        SceneManager.LoadScene("CardGame");
    }

    public void CardGameEnd()
    {
        round++;

        Board.main.refBoardUI.continuePrompt.SetActive(true);

        Board.main.refBoardUI.SetContinue(round <= roundsPerMatch);
    }

    public void RestartCardGame()
    {
        inCutscene = false;
        SceneManager.LoadScene("CardGame");
    }

    public void ResetMatch()
    {
        Debug.Log("Resetting Match");
        round = 1;
        matchScores = new int[] { 0, 0, 0, 0 };
        RestartCardGame();
    }

    public void EndMatch()
    {
        Debug.Log("Ending Match");
        Debug.Log("the match was " + match);
        match = match + 1;
        round = 1;
        inCutscene = false;
        matchScores = new int[] { 0, 0, 0, 0 };
        Debug.Log("the match was " + (match - 1) + " but is now " + match);
        AudioManager.main.cardTheme.Stop();
        AdvanceNarrative();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
