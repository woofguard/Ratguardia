using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board main; // static reference

    public Player[] players;
    public int turn; // which player's turn it is

    public int round; // what round it is
    public int numRounds; // how many rounds in a game

    public CardStack deck;

    private void Awake()
    {
        // board is a singleton
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        players = new Player[4];
        turn = 0; // might randomize this in the future or something

        // add each unique card to deck stack
        var cards = GetComponentsInChildren<Card>();
        foreach(Card card in cards)
        {
            deck.Push(card);
        }

        // shuffle deck
        deck.Shuffle();

        // add each player

        // deal each player 5 cards
    }

    // resolves a battle, returns index of which player won
    public int Battle()
    {
        return 0;
    }

    // calculates winner based on each player's score, returns player index
    public int DetermineWinner()
    {
        return 0;
    }
}
