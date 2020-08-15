using System;
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
    [HideInInspector] public Card winner; // card that wins the battle

    // characters
    public string player;
    public string aiType1;
    public string aiType2;
    public string aiType3;

    // references to player prefabs
    public GameObject humanPlayerPrefab;
    public GameObject AIPlayerPrefab;
    public GameObject networkPlayerPrefab;

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
        player = StateManager.main.combatants[0];
        aiType1 = StateManager.main.combatants[1];
        aiType2 = StateManager.main.combatants[2];
        aiType3 = StateManager.main.combatants[3];

        if(NetworkManager.main != null && NetworkManager.main.isNetworkGame && NetworkManager.main.isServer)
        {
            StartCoroutine(InitializeBoardServer());
        }
        else if(NetworkManager.main != null && NetworkManager.main.isNetworkGame && !NetworkManager.main.isServer)
        {
            StartCoroutine(InitializeBoardClient());
        }
        else
        {
            StartCoroutine(InitializeBoard());
        }     
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

        players = GenerateSinglePlayerGame(player, aiType1, aiType2, aiType3);

        yield return StartCoroutine(refBoardUI.DisplayBeginning());

        AudioManager.main.cardTheme.Play();
        AudioManager.main.sfxShuffle.Play();

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

    // initialize board as server, send initial board state to clients
    private IEnumerator InitializeBoardServer()
    {
        Debug.Log("setting up board as server");

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

        AudioManager.main.cardTheme.Play();
        AudioManager.main.sfxShuffle.Play();

        // shuffle deck, record each card in byte array
        byte[] deckPacket = deck.ShuffleServer();

        // send deck data to clients
        NetworkManager.main.SendToAllClients(deckPacket);

        // generate list of players, send to clients
        players = GenerateServerGame();
        for(int i = 1; i <= NetworkManager.main.numPlayers; i++)
        {
            byte[] playerPacket = CreatePlayerDataPacket(i);
            NetworkManager.main.serverSocket.Send(i, playerPacket);
            Debug.Log(BitConverter.ToString(playerPacket));
        }

        // deal out cards, player order is synced so it should turn out the same for clients
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < players.Length; j++)
            {
                players[j].Draw();
            }
        }

        yield return null;
    }

    // receive data from server and construct board from it
    private IEnumerator InitializeBoardClient()
    {
        Debug.Log("setting up board as client");

        turn = 0;
        scores = new int[4];

        // receive deck data from server

        // go thru deck packet and push each card to deck stack

        // receive player data from server

        // generate players from server data

        // deal out cards


        yield return null;
    }

    // creates the Player game objects for 1 human player and 3 AIs, returns array of players
    public Player[] GenerateSinglePlayerGame(string player, string ai1, string ai2, string ai3)
    {
        var humanPlayer = Instantiate(humanPlayerPrefab);
        var AIPlayer1 = Instantiate(AIPlayerPrefab);
        var AIPlayer2 = Instantiate(AIPlayerPrefab);
        var AIPlayer3 = Instantiate(AIPlayerPrefab);

        Player[] players = new Player[4];

        players[0] = humanPlayer.GetComponent<Player>();
        players[0].playerIndex = 0;
        players[0].SetNewCharacter(player);

        players[1] = AIPlayer1.GetComponent<Player>();
        players[1].playerIndex = 1;
        players[1].SetNewCharacter(ai1);
        (players[1] as AIPlayer).aiType = players[1].character.ai;
        players[1].transform.Translate(0f, 0.2f, 0f);

        players[2] = AIPlayer2.GetComponent<Player>();
        players[2].playerIndex = 2;
        players[2].SetNewCharacter(ai2);
        (players[2] as AIPlayer).aiType = players[2].character.ai;
        

        players[3] = AIPlayer3.GetComponent<Player>();
        players[3].playerIndex = 3;
        players[3].SetNewCharacter(ai3);
        (players[3] as AIPlayer).aiType = players[3].character.ai;
        players[3].transform.Translate(0f, 0.2f, 0f);

        return players;
    }

    // checks how many players are connected to network and creates network players for them
    // fills the rest in with AI players
    public Player[] GenerateServerGame()
    {
        Player[] players = new Player[4];

        // player 0 is human player for server
        players[0] = Instantiate(humanPlayerPrefab).GetComponent<Player>();
        players[0].playerIndex = 0;

        // create network players based on how many players are connected
        for(int i = 1; i < 4; i++)
        {
            // if enough players are connected
            if(i <= NetworkManager.main.numPlayers)
            {
                players[i] = Instantiate(networkPlayerPrefab).GetComponent<Player>();
                players[i].playerIndex = i;
            }
            // else create an ai player
            else
            {
                players[i] = Instantiate(AIPlayerPrefab).GetComponent<Player>();
                players[i].playerIndex = i;
            }
        }

        return players;
    }

    // assembles the packet that contains player info to send to the player
    // takes in the index of the client that is being sent to
    public byte[] CreatePlayerDataPacket(int playerIndex)
    {
        // 5 bytes for now, will later have to include player names/portraits
        byte[] packet = new byte[5];
        packet[0] = (byte)RMP.Players;

        for(int i = 1; i < packet.Length; i++)
        {
            // if index is the same, that is the human player
            if(playerIndex == i - 1)
            {
                packet[i] = (byte)RMP.Human;
            }
            else if(i - 1 <= NetworkManager.main.numPlayers)
            {
                packet[i] = (byte)RMP.Network;
            }
            else
            {
                // maybe let the players choose ai type later
                packet[i] = (byte)RMP.RandomAI;
            }
        }

        return packet;
    }

    public Player[] GenerateClientGame()
    {
        Player[] players = new Player[4];

        

        return players;
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

        for (int i = 0; i < scores.Length; i++)
        {
            StateManager.main.matchScores[i] += scores[i];
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

    // finds human player on board and returns them
    public HumanPlayer GetHumanPlayer()
    {
        foreach(var player in players)
        {
            if(player is HumanPlayer)
            {
                return (HumanPlayer)player;
            }
        }
        return null;
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
