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
    public int roundsPerMatch = 3;

    public string currentCutscene = "Intro";
    public bool inCutscene = false;

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
        combatants = new string[] { "The Child", "The Mother", "The Sibling", "The Father" };
        LoadCutscene("Intro");
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

    public void ExitGame()
    {
        Application.Quit();
    }
}
