using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Telepathy;

// our Trademarked Copyrighted Patented Ratguardia Message Protocol aka RMP(tm)(c)(r) /s
public enum RMP: byte
{
    // card game actions
    Draw = 0x1,      // standalone
    Discard = 0x2,   // followed by index of card to discard
    Steal = 0x3,     // standalone
    NoSteal = 0x4,   // standalone
    Combatant = 0x5, // followed by index of card sent into battle
    Player = 0x6,    // followed by board index of player

    // board/deck data
    Deck = 0xA1,     // standalone
    EndDeck = 0xA2,  // standalone
    Card = 0xA3,     // followed by card suit and card name

    // cards suits
    Chalices = 0xB1,
    Swords = 0xB2,
    Wands = 0xB3,
    Rings = 0xB4,

    // card names
    Assassin = 0xB5,
    Archer = 0xB6,
    Cavalier = 0xB7,
    Jester = 0xB8,
    King = 0xB9,
    Knight = 0xBA,
    Peasant = 0xBB,
    Preyrider = 0xBC,
    Queen = 0xBD,
    Witch = 0xBE,

    // game status/timing stuff?
    QueryPlayers = 0xC1, // client asks server if room is full (standalone)
    NumPlayers = 0xC2,   // server tells if room is full (followed by num players)
    StartGame = 0xC3
}

// manages network connection for both client & server players
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager main;

    // flags for whether the game is online and if the player is server/client
    [HideInInspector] public bool isNetworkGame = false;
    [HideInInspector] public bool isServer = false;

    // Telepathy TCP sockets (only one should ever be not null)
    public Server serverSocket;
    public Client clientSocket;

    // port number for TCP connection, should be Nice by default and if problems arise with it being taken
    // then the player should be able to change it
    public int port = 42069;

    // how many players are connected to the server
    [HideInInspector] public int numPlayers = 0; 

    // UI reference
    public NetworkUI ui;

    private void Awake()
    {
        // persistent singleton
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this.gameObject);

            // set telepathy logger output
            Telepathy.Logger.Log = Debug.Log;
            Telepathy.Logger.LogWarning = Debug.LogWarning;
            Telepathy.Logger.LogError = Debug.LogError;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // opens up socket for players to join
    public void HostGame()
    {
        // dont create socket if already open
        if(!isNetworkGame)
        {
            isNetworkGame = true;
            isServer = true;

            serverSocket = new Server();
            serverSocket.Start(port);

            StartCoroutine(ui.DisplayPlayerIPAddress());
        }
        else
        {
            Debug.Log("connection already open");
        }
    }

    // takes in host's ip address, opens socket and connects to host
    public void JoinGame()
    {
        // dont create socket if already open
        if(!isNetworkGame)
        {
            // get ip address from input field
            string ip = ui.inputField.text;

            clientSocket = new Client();
            clientSocket.Connect(ip, port);

            StartCoroutine(ui.UpdateConnectStatus(ip));
        }
        else
        {
            Debug.Log("connection already open");
        }    
    }

    // disconnects from network for either client or server
    public void CloseSocket()
    {
        if(isNetworkGame)
        {
            if(isServer)
            {
                serverSocket.Stop();
            }
            else
            {
                clientSocket.Disconnect();
            }
        }

        isNetworkGame = false;
        isServer = false;
    }

    // helper function to get host's ip address to share with clients
    public string GetIPv6Address()
    {
        // send a web request to an external webpage to find public ip
        // i just really love this url also
        string ipv6 = new WebClient().DownloadString("http://icanhazip.com");

        return ipv6;
    }

    public string GetIPv4Address()
    {
        // alternate website that give ipv4? will possibly test
        string ipv4 = new WebClient().DownloadString("http://ipinfo.io/ip").Trim();

        return ipv4;
    }

    // close sockets on quit just in case
    private void OnApplicationQuit()
    {
        CloseSocket();
    }


    // ****************************************
    // testing stuff
    // ****************************************
    public void TestSendClient()
    {
        serverSocket.Send(1, new byte[]{(byte)RMP.Draw});
    }

    public void TestSendServer()
    {
        clientSocket.Send(new byte[]{(byte)RMP.Draw});
    }

    // for testing, will probably not read messages in update later
    private void Update()
    {
        if(isNetworkGame)
        {
            if(isServer)
            {
                // show all new messages
                Message msg;
                while (serverSocket.GetNextMessage(out msg))
                {
                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            Debug.Log("Connected");
                            numPlayers++;
                            ui.UpdateNumPlayers();
                            break;
                        case Telepathy.EventType.Data:
                            Debug.Log("Data: " + BitConverter.ToString(msg.data));
                            break;
                        case Telepathy.EventType.Disconnected:
                            Debug.Log("Disconnected");
                            numPlayers--;
                            ui.UpdateNumPlayers();
                            break;
                    }
                }
            }
            else
            {
                // show all new messages
                Message msg;
                while (clientSocket.GetNextMessage(out msg))
                {
                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            Debug.Log("Connected");
                            break;
                        case Telepathy.EventType.Data:
                            Debug.Log("Data: " + BitConverter.ToString(msg.data));
                            break;
                        case Telepathy.EventType.Disconnected:
                            Debug.Log("Disconnected");
                            ui.ResetStatus();
                            break;
                    }
                }
            }
        }
    }
}
