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
        var socket = NetworkManager.main.serverSocket;

        yield return new WaitUntil(() => socket.listener != null);

        Debug.Log("Your IP Address: " + NetworkManager.main.GetIPAddress());

        // this is uncommented bc i cant figure out text mesh pro............
        // ipAddressText.text = ("Your IP Address: " + NetworkManager.main.GetIPAddress());
    }
}
