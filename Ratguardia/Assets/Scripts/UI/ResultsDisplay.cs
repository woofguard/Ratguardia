using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultsDisplay : MonoBehaviour
{
    public PlayerResult[] players;

    public void DisplayResults()
    {
        Result[] results = new Result[4];

        int i = 0;
        for(i = 0; i < results.Length; i++)
        {
            results[i].playerIndex = i;
            results[i].score = Board.main.scores[i];
        }

        // bubble sort the results
        // O(n^2) for 4 results is fine it's Fine it's. Fine
        for (i = 0; i < Board.main.players.Length - 1; i++)
        {
            for (int j = 0; j < Board.main.players.Length - i - 1; j++)
            {
                if (results[j].score < results[j + 1].score)
                {
                    Result temp = results[j];
                    results[j] = results[j + 1];
                    results[j + 1] = temp;
                }
            }
        }

        for(i = 0; i < results.Length; i++)
        {
            PopulatePlayerData(players[i], results[i]);
        }

    }

    void PopulatePlayerData(PlayerResult player, Result result)
    {
        Player p = Board.main.players[result.playerIndex];

        player.roundPoints.text = result.score + " points";
        player.totalPoints.text = result.score + " points"; // fix later to add total score

        // uncomment these when players have proper names/portraits
        // player.portrait = p.portrait;
        // player.charName = p.playerName;
        // player.line = ???

        for(int i = 0; i < 5; i++)
        {
            player.cards[i].DisplayCard(p.hand[i]);
        }
    }

    struct Result
    {
        public int playerIndex;
        public int score;
    }
}
