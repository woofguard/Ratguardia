﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    public NetworkUI nui;

    [SerializeField]
    Sprite[] portraits;
    int index = 0;

    public Image portrait;

    public TMP_InputField nameInput;
    public TextMeshProUGUI nameDisplay;

    private void Awake()
    {
        nui.userIcon = portraits[index];
        portrait.sprite = nui.userIcon;
        NetworkManager.main.portraits = this.portraits; // i need this actually
    }

    public void SetCharacterInfo()
    {
        nui.userName = nameInput.text;
        nui.userIcon = portraits[index];
        nameDisplay.text = nui.userName;

        // if server, set it directly
        if(NetworkManager.main.isServer)
        {
            NetworkManager.main.SetCharacterData(0, nui.userIcon, nui.userName);
        }
        // if client, send a packet
        else
        {
            NetworkManager.main.SendCharacterPacket(nui.userIcon, nui.userName);
        }
    }

    public void NextPortrait()
    {
        index++;
        if (portraits.Length <= index) index = 0;
        portrait.sprite = portraits[index];
        nui.userIcon = portraits[index];
        portrait.sprite = nui.userIcon;
    }

    public void PrevPortrait()
    {
        index--;
        if (index < 0) index = portraits.Length -1;
        portrait.sprite = portraits[index];
        nui.userIcon = portraits[index];
    }
}