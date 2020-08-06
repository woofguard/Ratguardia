using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultsDisplay : MonoBehaviour
{

    public PlayerResult[] players;

    // display results of current round
    public void DisplayMatchResults()
    {
       
        ResultsDisplay.Result[] results = new ResultsDisplay.Result[4];

        int i = 0;
        for (i = 0; i < results.Length; i++)
        {
            results[i].playerIndex = i;
            results[i].score = StateManager.main.matchScores[i];
        }

        // bubble sort the results
        // O(n^2) for 4 results is fine it's Fine it's. Fine It's really. fine
        for (i = 0; i < Board.main.players.Length - 1; i++)
        {
            for (int j = 0; j < Board.main.players.Length - i - 1; j++)
            {
                if (results[j].score < results[j + 1].score)
                {
                    ResultsDisplay.Result temp = results[j];
                    results[j] = results[j + 1];
                    results[j + 1] = temp;
                }
            }
        }

        for (i = 0; i < results.Length; i++)
        {
            PopulatePlayerData(players[i], results[i], i);
        }

    }

    // fill out player information for the round ie cards, points, dialogue
    void PopulatePlayerData(PlayerResult player, ResultsDisplay.Result result, int place)
    {
        Player p = Board.main.players[result.playerIndex];

        player.totalPoints.text = StateManager.main.matchScores[result.playerIndex] + " points";

        // uncomment these when players have proper names/portraits
        player.portrait.sprite = p.character.portrait;
        player.charName.text = p.character.title;

        string dialogue = "";
        switch (place)
        {
            case 0:
                dialogue = p.character.dialogue.match1st[0];
                break;
            case 1:
            case 2:
                dialogue = p.character.dialogue.match2nd[0];
                break;
            case 3:
                dialogue = p.character.dialogue.matchlast[0];
                break;
            default:
                break;
        }
        player.line.text = dialogue;
    }
}
