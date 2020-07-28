using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public static StateManager main;

    private void Awake()
    {
        // persistent singleton
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CardGameEnd()
    {
        Board.main.refBoardUI.restartPrompt.SetActive(true);
    }

    public void RestartCardGame()
    {
        SceneManager.LoadScene("CardGame");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
