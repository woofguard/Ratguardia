﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Telepathy;

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

    // temporary storage for other scripts to read from
    [HideInInspector] public byte[] packet;

    // makes sure steal packets go to the right person
    [HideInInspector] public byte[][] stealPackets;
    [HideInInspector] public bool[] packetRecieved;

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

            // initialize arrays
            stealPackets = new byte[4][];
            packetRecieved = new bool[4];
            for(int i = 0; i < packetRecieved.Length; i++)
            {
                packetRecieved[i] = false;
            }
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
            StartCoroutine(WaitForGameStart());
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
            isNetworkGame = true;
            isServer = true;

            // get ip address from input field
            string ip = ui.inputField.text;

            clientSocket = new Client();
            clientSocket.Connect(ip, port);

            StartCoroutine(ui.UpdateConnectStatus(ip));
            StartCoroutine(WaitForServerStart());
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

    // as server, takes a packet and sends it to all players
    public void SendToAllClients(byte[] packet)
    {
        for(int i = 1; i <= numPlayers; i++)
        {
            serverSocket.Send(i, packet);
        }
    }

    // when recieving a packet as server, sends to the rest of the players besides the one who sent it
    public void ForwardPacket(byte[] packet)
    {
        if(isServer)
        {
            for(int i = 1; i <= numPlayers; i++)
            {
                // dont send to the player whos turn it is, they already know
                if(i != Board.main.turn)
                {
                    serverSocket.Send(i, packet);
                }
            }
        } 
    }

    // server waits until start button is pressed
    private IEnumerator WaitForGameStart()
    {
        while(isNetworkGame && !ui.GameStarted())
        {
            // recieve connect/disconnect messages and update number of players
            Message msg;
            if(serverSocket.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Debug.Log("Connected");
                        
                        if(numPlayers < 4)
                        {
                          numPlayers++;  
                        }
                        
                        ui.UpdateNumPlayers();
                        break;
                    case Telepathy.EventType.Data:
                        string data = BitConverter.ToString(msg.data);
                        Debug.Log("Data: " + data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log("Disconnected");
                        numPlayers--;
                        ui.UpdateNumPlayers();
                        break;
                }
            }
            yield return null;
        }
        if(!isNetworkGame) yield break;
        // send clients message that game started
        SendToAllClients(new byte[]{((byte)RMP.StartGame)});
        Debug.Log("Starting multiplayer");
        StateManager.main.StartMultiplayer();
    }

    // client waits until server sends game start message
    private IEnumerator WaitForServerStart()
    {
        bool started = false;

        // read messages until one of them says the game started
        while(isNetworkGame && !started)
        {
            Message msg;
            if(clientSocket.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    // if the message says to start the game, do so
                    case Telepathy.EventType.Data:
                        if((RMP)msg.data[0] == RMP.StartGame)
                        {    
                            started = true;
                            StateManager.main.StartMultiplayer();
                        }
                        break;
                    // if disconnected from the host, reset the ui
                    case Telepathy.EventType.Disconnected:
                        Debug.Log("Disconnected");
                        ui.ResetStatus();
                        break;
                }
            }
            yield return null;
        } 
    }

    // waits to recieve to send a certain type of data packet
    public IEnumerator WaitForPacket(RMP data)
    {
        bool received = false;

        // read messages until deck message is recieved
        while(isNetworkGame && !received)
        {
            Message msg;
            // listen on different sockets based on whether player is server/client
            if(isServer)
            {
                if(serverSocket.GetNextMessage(out msg))
                {
                    if(msg.eventType == Telepathy.EventType.Data && (RMP)msg.data[0] == data)
                    {
                        received = true;
                        packet = msg.data;
                        ForwardPacket(packet);
                    }
                }
            }
            else
            {
                if(clientSocket.GetNextMessage(out msg))
                {
                    if(msg.eventType == Telepathy.EventType.Data && (RMP)msg.data[0] == data)
                    {
                        received = true;
                        packet = msg.data;
                    }
                }
            }
            yield return null;
        }
    }

    // overload that waits to recieve one of two types of packets
    public IEnumerator WaitForPacket(RMP data1, RMP data2)
    {
        bool received = false;

        // read messages until deck message is recieved
        while(isNetworkGame && !received)
        {
            Message msg;
            // listen on different sockets based on whether player is server/client
            if(isServer)
            {
                if(serverSocket.GetNextMessage(out msg))
                {
                    if(msg.eventType == Telepathy.EventType.Data)
                    {
                        // either the first type or second type
                        if((RMP)msg.data[0] == data1 || (RMP)msg.data[0] == data2)
                        {
                            received = true;
                            packet = msg.data;
                            ForwardPacket(packet);
                            ParseStealPacket(packet);
                        }
                    }
                }
            }
            else
            {
                if(clientSocket.GetNextMessage(out msg))
                {
                    if(msg.eventType == Telepathy.EventType.Data)
                    {
                        // either the first type or second type
                        if((RMP)msg.data[0] == data1 || (RMP)msg.data[0] == data2)
                        {
                            received = true;
                            packet = msg.data;
                            ParseStealPacket(packet);
                        }
                    }
                }
            }
            yield return null;
        }
    }

    // puts a steal packet in the correct part of the array for right player to read
    private void ParseStealPacket(byte[] packet)
    {
        // if it is a steal packet
        if(packet[0] == (byte)RMP.Steal || packet[0] == (byte)RMP.NoSteal)
        {
            // put it in the right place
            int index = packet[2];
            stealPackets[index] = packet;
            packetRecieved[index] = true;
        }
    }

    // close sockets on quit just in case
    private void OnApplicationQuit()
    {
        CloseSocket();
    }
}
