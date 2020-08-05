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
            PopulatePlayerData(players[i], results[i], i);
        }

    }

    void PopulatePlayerData(PlayerResult player, Result result, int place)
    {
        Player p = Board.main.players[result.playerIndex];

        player.roundPoints.text = result.score + " points";
        player.totalPoints.text = result.score + " points"; // fix later to add total score

        // uncomment these when players have proper names/portraits
        player.portrait.sprite = p.character.portrait;
        player.charName.text = p.character.title;

        string dialogue = "";
        switch(place)
        {
            case 0:
                dialogue = p.character.dialogue.round1st[0];
                break;
            case 1:
            case 2:
                dialogue = p.character.dialogue.round2nd[0];
                break;
            case 3:
                dialogue = p.character.dialogue.roundlast[0];
                break;
            default:
                break;
        }
        player.line.text = dialogue;

        for(int i = 0; i < 5; i++)
        {
            player.cards[i].SetOwnerDisplay(false);
            player.cards[i].DisplayCard(p.hand[i]);
        }
    }

    struct Result
    {
        public int playerIndex;
        public int score;
    }
}
