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
        isNetworkGame = true;
        isServer = true;

        serverSocket = new Server();
        serverSocket.Start(port);

        StartCoroutine(ui.DisplayPlayerIPAddress());
    }

    // takes in host's ip address, opens socket and connects to host
    public void JoinGame()
    {
        isNetworkGame = true;
        isServer = false;
    }

    // disconnects from network for either client or server
    public void CloseSocket()
    {
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
}
