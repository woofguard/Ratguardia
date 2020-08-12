using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NetworkUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI ipAddressText;
    public TextMeshProUGUI joinStatus;
    public TextMeshProUGUI hostStatus;

    // this needs to be a coroutine bc otherwise itll run before the socket is finished being
    // created and give a null reference exception bc telepathy is actually multithreaded
    public IEnumerator DisplayPlayerIPAddress()
    {
        hostStatus.text = "Status: connecting...";

        var socket = NetworkManager.main.serverSocket;

        yield return new WaitUntil(() => socket.listener != null);

        string ip = NetworkManager.main.GetIPAddress();
        Debug.Log("Your IP Address: " + ip);

        // text mesh pro................
        ipAddressText.text = ("Your IP Address: " + ip);
        UpdateNumPlayers();
    }

    // displays how many playeres are connected to the server
    public void UpdateNumPlayers()
    {
        hostStatus.text = ("Status: Hosting server \n" + NetworkManager.main.numPlayers + " players joined");
    }

    // updates text based on whether connection was successful
    public IEnumerator UpdateConnectStatus(string ip)
    {
        // wait until client is done connecting
        joinStatus.text = "Status: connecting...";
        yield return new WaitUntil(() => !NetworkManager.main.clientSocket.Connecting);

        // if connection was successful
        if(NetworkManager.main.clientSocket.Connected)
        {
            joinStatus.text = "Status: Joined server at " + ip + "\nWaiting for host...";

            // update network manager fields
            NetworkManager.main.isNetworkGame = true;
            NetworkManager.main.isServer = false;
        }
        else
        {
            joinStatus.text = "Status: Failed to connect to server";
        }
    }

    // resets text fields
    public void ResetStatus()
    {
        joinStatus.text = "Status: not joined";
        hostStatus.text = "Status: not hosting";
        ipAddressText.text = "Your IP Address: ";
    }
}
