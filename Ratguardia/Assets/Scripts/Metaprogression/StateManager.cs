using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public static StateManager main;

    public int[] matchScores;

    public int round;

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

    public void CardGameEnd()
    {
        round++;

        if(round <= 3) Board.main.refBoardUI.restartPrompt.SetActive(true);
    }

    public void RestartCardGame()
    {
        SceneManager.LoadScene("CardGame");
    }

    public void ResetMatch()
    {
        round = 1;
        matchScores = new int[] { 0, 0, 0, 0 };
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
