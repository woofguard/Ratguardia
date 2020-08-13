using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public static StateManager main;

    public int[] matchScores;

    public int match = 0;
    public string[] combatants;

    public int round;
    public int roundsPerMatch = 1;

    public string currentCutscene = "Intro";
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
            matchScores = new int[] { 0, 0, 0, 0 };
            Debug.Log(SceneManager.GetActiveScene().name);
            if (SceneManager.GetActiveScene().name == "Cutscene") LoadTutorial();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadCutscene(string scene)
    {
        inCutscene = true;
        currentCutscene = scene;
        SceneManager.LoadScene("Cutscene");
    }

    public void LoadTutorial()
    { 
        LoadCutscene("Intro");
    }

    public void AdvanceNarrative()
    {
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
                    break;
                case 4:
                    break;
                default:
                    break;
            }
            RestartCardGame();
        }
        else
        {
            inCutscene = true;
            string cutscene = "Intro";
            switch (match)
            {
                case 0: // i don't think this should happen
                    break;
                case 1:
                    cutscene = "Intro";
                    break;
                case 2:
                case 3:
                    cutscene = "Intro";
                    break;
                case 4:
                    cutscene = "Intro";
                    break;
                default:
                    break;
            }
            if (!(cutscene == "")) LoadCutscene(cutscene);
            else RestartCardGame();
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
        match++;
        round = 1;
        matchScores = new int[] { 0, 0, 0, 0 };
        AdvanceNarrative();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
