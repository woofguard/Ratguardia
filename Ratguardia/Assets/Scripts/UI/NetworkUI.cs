using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NetworkUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI ipv6Text;
    public TextMeshProUGUI ipv4Text;
    public TextMeshProUGUI joinStatus;
    public TextMeshProUGUI hostStatus;
    public Button startButton;

    private bool buttonPressed = false;

    private void Awake()
    {
        NetworkManager.main.ui = this;
    }

    // this needs to be a coroutine bc otherwise itll run before the socket is finished being
    // created and give a null reference exception bc telepathy is actually multithreaded
    public IEnumerator DisplayPlayerIPAddress()
    {
        hostStatus.text = "Status: connecting...";

        var socket = NetworkManager.main.serverSocket;

        yield return new WaitUntil(() => socket.listener != null);

        string ipv6 = NetworkManager.main.GetIPv6Address();
        string ipv4 = NetworkManager.main.GetIPv4Address();

        // text mesh pro................
        ipv6Text.text = ipv6;
        ipv4Text.text = ipv4;
        UpdateNumPlayers();

        startButton.gameObject.SetActive(true);
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
        ipv6Text.text = "";
        ipv4Text.text = "";
    }

    // functions to copy ip to clipboard
    public void CopyIPv6()
    {
        TextEditor te = new TextEditor();
        te.text = ipv6Text.text.Trim();
        te.SelectAll();
        te.Copy();
    }

    public void CopyIPv4()
    {
        TextEditor te = new TextEditor();
        te.text = ipv4Text.text.Trim();
        te.SelectAll();
        te.Copy();
    }

    public bool GameStarted()
    {
        if(buttonPressed)
        {
            buttonPressed = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ButtonPressed()
    {
        buttonPressed = true;
    }

    // these button functions need to be in here actually bc the network manager hookup might not be the right one
    public void HostButton()
    {
        NetworkManager.main.HostGame();
    }

    public void JoinButton()
    {
        NetworkManager.main.JoinGame();
    }

    public void CancelButton()
    {
        NetworkManager.main.CloseSocket();
        ResetStatus();
    }
}
