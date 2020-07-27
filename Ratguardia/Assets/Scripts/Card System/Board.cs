using System.Collections;
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

    // references to player prefabs
    public GameObject refHumanPlayer;
    public GameObject refAIPlayer;

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
            refBoardUI.DisplayScores();
        }
        else
        {
            Debug.Log("Player " + player + "'s turn");
            StartCoroutine(players[player].TakeTurn()); 
        }
    }

    // resolves a battle, returns the winning player's card
    public Card Battle(List<Card> combatants)
    {
        Card winner = combatants[0];

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

        // yield return StartCoroutine(refBoardUI.DisplayBattle(combatants, winner));

        // reset combatant data for all players
        foreach(Player player in players)
        {
            player.combatant = null;
        }

        return winner;
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
