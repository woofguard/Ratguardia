using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeginRoundDisplay : MonoBehaviour
{
    public PlayerResult[] players;
    public TextMeshProUGUI roundNum;
    public GameObject beginPrompt;

    // can i just disable this in an online game... will this work....
    private void Start()
    {
        if(NetworkManager.main.isNetworkGame)
        {
            gameObject.SetActive(false);
            beginPrompt.SetActive(false);
        }
    }

    // display results of current round
    public void DisplayNewRound()
    {
        roundNum.text = "Round " + StateManager.main.round;

        for(int i = 0; i < Board.main.players.Length; i++)
        {
            PopulatePlayerData(players[i], i);
        }

    }

    // fill out player information for the round ie cards, points, dialogue
    void PopulatePlayerData(PlayerResult player, int place)
    {
        Player p = Board.main.players[place];

        // uncomment these when players have proper names/portraits
        player.portrait.sprite = p.character.portrait;
        player.charName.text = p.character.title;

        string dialogue = "";
        switch(place)
        {
            case 0:
                dialogue = p.character.dialogue.intro[0];
                break;
            case 1:
            case 2:
                dialogue = p.character.dialogue.intro[0];
                break;
            case 3:
                dialogue = p.character.dialogue.intro[0];
                break;
            default:
                break;
        }
        player.line.text = dialogue;
    }
}
