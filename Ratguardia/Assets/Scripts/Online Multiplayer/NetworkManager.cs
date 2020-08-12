﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Telepathy;

// our Trademarked Copyrighted Patented Ratguardia Message Protocol aka RMP(tm)(c)(r) /s
public enum RMP: byte
{
    Draw = 0x1,      // standalone
    Discard = 0x2,   // followed by index of card to discard
    Steal = 0x3,     // standalone
    NoSteal = 0x4,   // standalone
    Combatant = 0x5, // followed by index of card sent into battle
    Player = 0x6     // followed by board index of player
}

// manages network connection for both client & server players
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager main;

    // flags for whether the game is online and if the player is server/client
    [HideInInspector] public bool isNetworkGame;
    [HideInInspector] public bool isServer;

    // Telepathy TCP sockets (only one should ever be not null)
    public Server serverSocket;
    public Client clientSocket;

    // port number for TCP connection, should be Nice by default and if problems arise with it being taken
    // then the player should be able to change it
    public int port;

    // maps Telepathy connection id to player index in board state
    public Dictionary<int, int> players;

    // how many players are connected to the server
    public int numPlayers = 0; 

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

            // set initial flag values
            isNetworkGame = false;
            isServer = false;
            port = 42069;
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
    public string GetIPAddress()
    {
        // get a list of local ip addresses
        var ipList = Dns.GetHostAddresses(Dns.GetHostName());
        // foreach(var ip in ipList)
        // {
        //     Debug.Log(ip.ToString());
        // }

        // just return the first address for now??
        return ipList[0].ToString();
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
                            break;
                    }
                }
            }
        }
    }
}
