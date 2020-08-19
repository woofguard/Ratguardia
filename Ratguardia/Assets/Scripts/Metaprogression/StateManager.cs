using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public static StateManager main;

    public static int roundNum;
    public int round
    {
        get { return StateManager.roundNum; }
    }
    public static int roundsPerMatch = 3;

    public static int match;
    public int[] matchScores;

    public string[] combatants;
    public Stack<string> replacements;

    public static int charDeath = -1;
    public static int lastVictor = -1;

    public static string currentCutscene;
    public bool inCutscene = false;

    public static string background = "";

    private void Awake()
    {
        // persistent singleton
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this.gameObject);
            match = 0;
            roundNum = 1;
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
                    roundsPerMatch = 1;
                    background = "Table";
                    break;
                case 1:
                    combatants = new string[] { "The Jester", "The Peasant", "The Knight", "The Cavalier" };
                    roundsPerMatch = 2;
                    background = "TableCarvings";
                    break;
                case 2:
                case 3:
                    if (charDeath > -1) ReplaceCombatant(charDeath);
                    charDeath = -1;
                    background = "TableCarvings";
                    break;
                case 4:
                    ReplaceCombatant(2, "The King");
                    roundsPerMatch = 3;
                    background = "Tablecloth";
                    break;
                case 5:
                    background = "";
                    ReturnToTitle();
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
            background = "Pattern";
            switch (match)
            {
                case 0: // i don't think this should happen
                    break;
                case 1:
                    currentCutscene = "Pre Match 1";
                    break;
                case 2:
                    if (charDeath == -1) currentCutscene = "Match1(Spare)";
                    else currentCutscene = "Match1(Feed)";
                    break;
                case 3:
                    if (charDeath == -1)
                    {
                        if (replacements.Count == 2) currentCutscene = "Match2(Spare)";
                        else currentCutscene = "Match2(SpareAfterFeed)";
                    }
                    else
                    {
                        if(replacements.Count == 2) currentCutscene = "Match1(Feed)";
                        else currentCutscene = "Match2(Feed)";
                    }
                    break;
                case 4:
                    if (charDeath == -1) currentCutscene = "Match3(Spare)";
                    else currentCutscene = "Match3(Feed)";
                    break;
                case 5:
                    currentCutscene = "Victory";
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
        roundNum = roundNum + 1;

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
        roundNum = 1;
        matchScores = new int[] { 0, 0, 0, 0 };
        RestartCardGame();
    }

    public void EndMatch()
    {
        Debug.Log(NetworkManager.main.isNetworkGame + " " + charDeath + " " + match);
        if (!NetworkManager.main.isNetworkGame && charDeath == 0 && match >= 1)
        {
            Board.main.refBoardUI.DisplayGameOver(); // display gameOver if player dies
            roundNum = 1;
            return;
        }

        match = match + 1;
        roundNum = 1;
        inCutscene = false;
        matchScores = new int[] { 0, 0, 0, 0 };
        AudioManager.main.cardTheme.Stop();

        AdvanceNarrative();
    }

    public void RetryMatch()
    {
        Debug.Log("Retrying Match");
        Debug.Log("the match was " + match);
        AudioManager.main.cardTheme.Stop();
        ResetMatch();
    }

    public void LoadNetworkTest()
    {
        SceneManager.LoadScene("NetworkTest");
    }

    public void ReturnToTitle()
    {
        match = 0;
        roundNum = 1;
        combatants = new string[] { "The Jester", "The Peasant", "The Knight", "The Cavalier" };

        replacements = new Stack<string>();
        replacements.Push("The Preyrider");
        replacements.Push("The Assassin");

        matchScores = new int[] { 0, 0, 0, 0 };
        currentCutscene = "Intro";

        Destroy(AudioManager.main.gameObject);
        SceneManager.LoadScene("TitleScreen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
