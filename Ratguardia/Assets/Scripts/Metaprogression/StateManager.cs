using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public static StateManager main;

    public int[] matchScores;

    public int round;
    public int roundsPerMatch = 3;

    private void Awake()
    {
        // persistent singleton
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this.gameObject);

            round = 1;
            matchScores = new int[] { 0, 0, 0, 0 };
        }
        else
        {
            Destroy(gameObject);
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
