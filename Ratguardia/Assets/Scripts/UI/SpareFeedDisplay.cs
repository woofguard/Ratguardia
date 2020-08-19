using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpareFeedDisplay : MonoBehaviour
{
    public PlayerResult[] players;

    public TMPro.TextMeshProUGUI decision;

    private void Awake()
    {
        DisplaySpareFeed();
    }

    // display who spares and who fed
    public void DisplaySpareFeed()
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

        PopulatePlayerData(players[0], results[0], 0);
        PopulatePlayerData(players[3], results[3], 3);

        if(players[0].charName.text == "The Jester")
        {
            decision.text = "Spare";
            players[0].line.text = Board.main.players[0].character.dialogue.spare;
            players[3].line.text = Board.main.players[results[3].playerIndex].character.dialogue.spared;
            StateManager.charDeath = -1;
        }
        else
        {
            decision.text = "Feed";
            players[0].line.text = Board.main.players[results[0].playerIndex].character.dialogue.feed;
            players[3].line.text = Board.main.players[results[3].playerIndex].character.dialogue.death;
            StateManager.charDeath = results[3].playerIndex;
        }
        StateManager.lastVictor = results[0].playerIndex;

    }

    // fill out player information for the round ie cards, points, dialogue
    void PopulatePlayerData(PlayerResult player, ResultsDisplay.Result result, int place)
    {
        Player p = Board.main.players[result.playerIndex];


        // uncomment these when players have proper names/portraits
        player.portrait.sprite = p.character.portrait;
        player.charName.text = p.character.title;
    }
}
