using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board main; // static reference

    public Player[] players;
    private int turn; // which player's turn it is

    public CardStack deck;
    public CardStack rubblePile;

    // references to player prefabs
    public GameObject refHumanPlayer;
    public GameObject refAIPlayer;

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
        StartCoroutine(InitializeBoard());
    }

    private IEnumerator InitializeBoard()
    {
        Debug.Log("setting up board");

        players = new Player[4];
        turn = 0; // might randomize this in the future or something

        // add each unique card to deck stack
        var cards = GetComponentsInChildren<Card>();
        foreach(Card card in cards)
        {
            card.owner = -1;
            deck.Push(card);
        }

        deck.Shuffle();

        players = GenerateSinglePlayerGame();

        // deal each player 5 cards
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < players.Length; j++)
            {
                players[j].Draw();
            }
        }

        Debug.Log("Cards dealt, num remaining in deck: " + deck.stack.Count);

        GiveTurn(turn);

        yield return null;
    }

    // gives control to the player whose turn it is
    public void GiveTurn(int player)
    {
        turn = player;
        players[player].hasTurn = true;
        players[PrevPlayer()].hasTurn = false;
        
        // if the deck is empty, end the game
        if(deck.stack.Count <= 0)
        {
            DetermineWinner();
        }
        else
        {
            Debug.Log("Player " + player + "'s turn");
            StartCoroutine(players[player].TakeTurn()); 
        }
    }

    // resolves a battle, returns index of which player won
    public int Battle()
    {
        return 0;
    }

    // calculates winner based on each player's score, returns player index
    public int DetermineWinner()
    {
        Debug.Log("Player " + turn + " wins!");
        return 0;
    }

    // creates the Player game objects for 1 human player and 3 AIs, returns array of players
    public Player[] GenerateSinglePlayerGame()
    {
        var humanPlayer = Instantiate(refHumanPlayer);
        var AIPlayer1 = Instantiate(refAIPlayer);
        var AIPlayer2 = Instantiate(refAIPlayer);
        var AIPlayer3 = Instantiate(refAIPlayer);

        Player[] players = new Player[4];

        players[0] = humanPlayer.GetComponent<Player>();
        players[0].playerIndex = 0;
        players[1] = AIPlayer1.GetComponent<Player>();
        players[1].playerIndex = 1;
        players[2] = AIPlayer2.GetComponent<Player>();
        players[2].playerIndex = 2;
        players[3] = AIPlayer3.GetComponent<Player>();
        players[3].playerIndex = 3;

        return players;
    }

    // helper functions to return which player is next/previous in the turn order
    public int NextPlayer()
    {
        return (turn + 1) % players.Length;
    }

    public int PrevPlayer()
    {
        if(turn == 0)
        {
            return 3;
        }
        else
        {
            return turn - 1;
        }
    }
}
