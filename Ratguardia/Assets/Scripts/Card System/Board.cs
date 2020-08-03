﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board main; // static reference

    public Player[] players;
    private int turn; // which player's turn it is
    public int[] scores;

    public CardStack deck;
    public CardStack rubblePile;
    [HideInInspector] public Card winner; // card that wins the battle

    // references to player prefabs
    public GameObject humanPlayerPrefab;
    public GameObject AIPlayerPrefab;

    // reference to board UI
    public BoardUIManager refBoardUI;

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
        scores = new int[4];

        // add each unique card to deck stack
        var cards = GetComponentsInChildren<Card>();
        foreach(Card card in cards)
        {
            card.owner = -1;
            deck.Push(card);
        }

        AudioManager.main.sfxShuffle.Play();
        deck.Shuffle();

        players = GenerateSinglePlayerGame("basic", "random", "random");

        // dont play draw sfx when dealing cards
        AudioManager.main.sfxDraw.mute = true;

        // deal each player 5 cards
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < players.Length; j++)
            {
                players[j].Draw();
            }
        }

        // Debug.Log("Cards dealt, num remaining in deck: " + deck.stack.Count);

        yield return new WaitForSeconds(0.25f);
        AudioManager.main.sfxDraw.mute = false;
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
            refBoardUI.DisplayScores();
            StateManager.main.CardGameEnd();
        }
        else
        {
            Debug.Log("Player " + player + "'s turn");
            StartCoroutine(players[player].TakeTurn()); 
        }
    }

    // resolves a battle, returns the winning player's card
    public IEnumerator Battle(List<Card> combatants)
    {
        winner = combatants[0];

        for(int i = 1; i < combatants.Count; i++)
        {
            // if the card has higher attack
            if(combatants[i].atk > winner.atk)
            {
                winner = combatants[i];
            }
            // if they have the same attack, take the higher suit
            else if(combatants[i].atk == winner.atk && combatants[i].suit < winner.suit)
            {
                winner = combatants[i];
            }
        }

        yield return StartCoroutine(refBoardUI.DisplayBattle(combatants, winner));

        // reset combatant data for all players
        foreach(Player player in players)
        {
            player.combatant = null;
        }
    }

    // calculates winner based on each player's score, returns player index
    public int DetermineWinner()
    {
        // calculate every player's score
        for(int i = 0; i < scores.Length; i++)
        {
            players[i].FlipHand();
            scores[i] = players[i].CalculateScore();
        }

        // if any player has a jester, activate its effect
        Card jester = FindJester();
        if(jester != null)
        {
            jester.ActivateEffect();
        }

        // find every witch and activate its effect
        List<Card> witches = FindWitches();
        foreach(Card witch in witches)
        {
            witch.ActivateEffect();
        }

        // find the player with the highest score
        int winner = 0;
        for(int i = 0; i < scores.Length; i++)
        {
            Debug.Log("Player " + i + " score: " + scores[i]);
            if(scores[i] > scores[winner])
            {
                winner = i;
            }
        }

        Debug.Log("Player " + winner + " wins!");
        return winner;
    }

    // checks if any player has a jester in their hand
    public Card FindJester()
    {
        // dreaded n^2..... it will only ever look at like 20 cards total so its ok tho
        foreach(var player in players)
        {
            foreach(var card in player.hand)
            {
                if(card.cardName == "Jester")
                {
                    return card;
                }
            }
        }

        return null;
    }

    // count up all the witches in players' hands
    public List<Card> FindWitches()
    {
        List<Card> witches = new List<Card>();

        foreach(var player in players)
        {
            foreach(var card in player.hand)
            {
                if(card.cardName == "Witch")
                {
                    witches.Add(card);
                }
            }
        }

        return witches;
    }

    // creates the Player game objects for 1 human player and 3 AIs, returns array of players
    public Player[] GenerateSinglePlayerGame(string ai1, string ai2, string ai3)
    {
        var humanPlayer = Instantiate(humanPlayerPrefab);
        var AIPlayer1 = Instantiate(AIPlayerPrefab);
        var AIPlayer2 = Instantiate(AIPlayerPrefab);
        var AIPlayer3 = Instantiate(AIPlayerPrefab);

        Player[] players = new Player[4];

        players[0] = humanPlayer.GetComponent<Player>();
        players[0].playerIndex = 0;

        players[1] = AIPlayer1.GetComponent<Player>();
        players[1].playerIndex = 1;
        (players[1] as AIPlayer).aiType = ai1;

        players[2] = AIPlayer2.GetComponent<Player>();
        players[2].playerIndex = 2;
        (players[2] as AIPlayer).aiType = ai2;

        players[3] = AIPlayer3.GetComponent<Player>();
        players[3].playerIndex = 3;
        (players[3] as AIPlayer).aiType = ai3;

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

    // organize rubble cards visually
    public void OrganizeRubble()
    {
        foreach(Card card in rubblePile.stack)
        {
            card.visualCard.HideCard(); // hide all cards
            card.transform.localPosition = new Vector3(0f, 0f, 0f);
            card.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            card.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // bad hardcoding, don't look at me
        }
        rubblePile.stack.Peek().visualCard.UnhideCard(); // unhide the top card
    }
}
