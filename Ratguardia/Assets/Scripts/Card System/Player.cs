﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public List<Card> hand;
    public bool faceUp; // whether the hand is face up or not (true for human player)
    public bool revealed;

    [HideInInspector] public int score;
    [HideInInspector] public bool hasTurn;
    [HideInInspector] public bool isStealing;
    [HideInInspector] public int playerIndex; // which index this player is in the Board array
    [HideInInspector] public Card combatant;  // card fighting in battle, used for stealing
    [HideInInspector] public bool doneStealing = true;

    public GameObject fiveCardLayout;
    public GameObject sixCardLayout;

    public Character[] charList;
    public Character character;

    public GameObject icon;

    public void SetNewCharacter(string name)
    {
        Debug.Log("Searching for " + name);
        int i = 0;
        for(i = 0; i < charList.Length; i++)
        {
            if (charList[i].title == name)
            {
                character = charList[i];
                break;
            }
        }
        //if(i == charList.Length) character = transform.GetComponentInChildren<Character>();
        SetIcon();
    }

    public void SetIcon()
    {
        icon.GetComponentInChildren<SpriteRenderer>().sprite = character.portrait;
        icon.GetComponentInChildren<TMPro.TextMeshPro>().text = character.title;
    }

    // function containing player actions on their turn
    public abstract IEnumerator TakeTurn();

    public virtual IEnumerator EndTurn()
    {
        // if not in battle
        yield return new WaitWhile(() => Board.main.refBoardUI.stealUI.activeSelf);
        SendEndTurnPacket();
        Board.main.GiveTurn(Board.main.NextPlayer());
        icon.transform.Find("Outline").gameObject.SetActive(false);
        yield return null;
    }

    public virtual Card Draw()
    {
        if(!Board.main.initializing)
        {
            SendDrawPacket();
            AudioManager.main.sfxDraw.Play();
        }

        var newCard = Board.main.deck.Pop();
        Board.main.refBoardUI.UpdateDeckUI();

        // flip card to the same side as rest of hand
        newCard.Flip(faceUp);
        newCard.Reveal(revealed);

        // set card's owner to this player
        newCard.owner = playerIndex;
        
        hand.Add(newCard);
        newCard.transform.SetParent(transform);
        
        ArrangeHand();
        
        score = CalculateScore();

        return newCard;
    }

    // discard using a specific card reference
    public IEnumerator Discard(Card c)
    {
        SendDiscardPacket(c);

        Board.main.rubblePile.Push(c);
        c.transform.SetParent(Board.main.rubblePile.transform);
        Board.main.OrganizeRubble();
        hand.Remove(c);

        // set card's owner to -1 (no player)
        c.owner = -1;
        c.rubble = true;
        
        c.ResetStats();

        c.visualCard.FlipUp();

        AudioManager.main.sfxDiscard.Play();
        ArrangeHand();
        
        score = CalculateScore();
        
        yield return StartCoroutine(PromptSteal());
    }

    // discard using the cards index in the hand
    public IEnumerator Discard(int i)
    {
        Card c = hand[i];
        yield return StartCoroutine(Discard(c));
    }

    // steal a card from the rubble pile
    public void Steal()
    {
        Card stolen = Board.main.rubblePile.Pop();
        stolen.owner = playerIndex;
        hand.Add(stolen);
        stolen.visualCard.UpdateRubble();
        
        AudioManager.main.sfxDraw.Play();
        stolen.transform.SetParent(transform);
        ArrangeHand();
        
        score = CalculateScore();
    }

    // ask other players if they want to steal when you discard
    public IEnumerator PromptSteal()
    {
        yield return new WaitForSeconds(0.5f);
        List<Card> combatants = new List<Card>();
        
        // ask each player if they want to steal
        foreach(var player in Board.main.players)
        {
            // if not the player who discarded and player can steal
            if(player.playerIndex != this.playerIndex && player.CanSteal())
            {
                // ask concurrently now, wait for everyone to finish later
                StartCoroutine(player.DecideSteal());   
            }
        }

        yield return new WaitUntil(() => Board.main.PlayersDoneStealing());

        foreach(var player in Board.main.players)
        {
           // if player decides to steal, add card to combatants list
            if(player.combatant != null)
            {
                combatants.Add(player.combatant);
            } 
        }

        // if anyone wants to steal, run the battle
        if(combatants.Count > 0)
        {
            yield return StartCoroutine(Board.main.Battle(combatants));
            Card winner = Board.main.winner;
            Board.main.players[winner.owner].Steal();

            // if the winner used a cavalier, activate its effect
            if(winner.cardName == "Cavalier")
            {
                winner.ActivateEffect();
            }

            yield return new WaitForSeconds(0.75f);
            yield return StartCoroutine(Board.main.players[winner.owner].Discard(winner));
            Board.main.winner = null;
            yield return new WaitForSeconds(0.75f);
        }
    }

    // choose whether to steal
    public abstract IEnumerator DecideSteal();

    // checks conditions for stealing
    public bool CanSteal()
    {
        foreach(Card c in hand)
        {
            // player has a card that isn't rubble and has atk points
            if(c.atk > 0 && !c.rubble)
            {
                return true;
            }
        }

        return false;
    }

    // look thru player's hand and add up all the def scores
    public int CalculateScore()
    {
        score = 0; // reset before recalculating

        List<Card> kings = new List<Card>();

        foreach(Card card in hand)
        {
            // activate effects based on card name
            switch(card.cardName)
            {    
                case "Queen":
                    card.ActivateEffect();
                    break;
                case "Peasant":
                    card.ActivateEffect();
                    break;
                case "King":
                    kings.Add(card);
                    break;
                case "Knight":
                    card.ActivateEffect();
                    break;
            }
            score += card.def;
        }

        // activate king effects after initial score is added up
        foreach(Card king in kings)
        {
            king.ActivateEffect();
        }

        return score;
    }

    // flips entire hand face up at final score calculation
    public void FlipHand()
    {
        foreach(var card in hand)
        {
            card.Flip(true);
        }
    }

    public virtual void ArrangeHand()
    {
        Debug.Log("Arranging player " + playerIndex + "'s hand");
        if (fiveCardLayout == null || sixCardLayout == null) return;
        if (hand.Count == 5)
        {
            for (int i = 0; i < 5; i++)
            {
                Transform card = fiveCardLayout.transform.GetChild(i);
                hand[i].transform.localPosition = card.localPosition;
                hand[i].transform.localRotation = card.localRotation;
                hand[i].transform.localScale = card.localScale;
            }
            Vector3 iconPos = sixCardLayout.transform.GetChild(0).position;
            iconPos.x -= 1.05f;
            iconPos.y -= 0.2f;
            icon.transform.localPosition = iconPos;
            icon.transform.localScale = new Vector3(0.65f, 0.65f, 1.0f);
        }
        else if (hand.Count == 6)
        {
            for (int i = 0; i < 6; i++)
            {
                Transform card = sixCardLayout.transform.GetChild(i);
                hand[i].transform.localPosition = card.localPosition;
                hand[i].transform.localRotation = card.localRotation;
                hand[i].transform.localScale = card.localScale;
            }
            Vector3 iconPos = sixCardLayout.transform.GetChild(0).position;
            iconPos.x -= 2.15f;
            iconPos.y -= 0.2f;
            icon.transform.localPosition = iconPos;
            icon.transform.localScale = new Vector3(0.65f, 0.65f, 1.0f);
        }
    }

    // if game is online, send data to other players that player drew
    protected void SendDrawPacket()
    {
        if(NetworkManager.main.isNetworkGame && this is HumanPlayer)
        {
            // literally send one byte
            byte[] packet = new byte[1];
            packet[0] = (byte)RMP.Draw;

            // whether player is client/server
            if(NetworkManager.main.isServer)
            {
                NetworkManager.main.SendToAllClients(packet);
            }
            else
            {
                NetworkManager.main.clientSocket.Send(packet);
            }        
        }
    }

    // if game is online, tell players what card was discarded
    protected void SendDiscardPacket(Card card)
    {
        if(NetworkManager.main.isNetworkGame && this is HumanPlayer)
        {
            // discard byte, index byte
            byte[] packet = new byte[2];
            packet[0] = (byte)RMP.Discard;

            // look for index of card in hand
            int index = hand.IndexOf(card);
            packet[1] = Convert.ToByte(index);

            // whether player is client/server
            if(NetworkManager.main.isServer)
            {
                NetworkManager.main.SendToAllClients(packet);
            }
            else
            {
                NetworkManager.main.clientSocket.Send(packet);
            }  
        }
    }

    // if game is online, tell players that player is stealing, and with which card
    protected void SendStealPacket()
    {
        if(NetworkManager.main.isNetworkGame)
        {
            // steal byte, player byte, index byte, combatant byte, index byte
            byte[] packet = new byte[5];
            packet[0] = (byte)RMP.Steal;
            packet[1] = (byte)RMP.Player;
            packet[3] = (byte)RMP.Combatant;

            // convert player index & combatant index to bytes
            packet[2] = Convert.ToByte(playerIndex);
            packet[4] = Convert.ToByte(hand.IndexOf(combatant));

            // send to client/server
            if(NetworkManager.main.isServer)
            {
                NetworkManager.main.SendToAllClients(packet);
            }
            else
            {
                NetworkManager.main.clientSocket.Send(packet);
            }
        }
    }

    // if game is online, tell players that player is not stealing
    protected void SendNoStealPacket()
    {
        if(NetworkManager.main.isNetworkGame)
        {
            // steal byte, player byte, index byte
            byte[] packet = new byte[3];
            packet[0] = (byte)RMP.NoSteal;
            packet[1] = (byte)RMP.Player;

            // convert player index to byte
            packet[2] = Convert.ToByte(playerIndex);

            // send to client/server
            if(NetworkManager.main.isServer)
            {
                NetworkManager.main.SendToAllClients(packet);
            }
            else
            {
                NetworkManager.main.clientSocket.Send(packet);
            }
        }
    }

    // if game is online, tell players that turn is over
    protected void SendEndTurnPacket()
    {
        if(NetworkManager.main.isNetworkGame && this is HumanPlayer)
        {
            // literally just 1 byte again
            byte[] packet = new byte[1];
            packet[0] = (byte)RMP.EndTurn;

            // send to client/server
            if(NetworkManager.main.isServer)
            {
                NetworkManager.main.SendToAllClients(packet);
            }
            else
            {
                NetworkManager.main.clientSocket.Send(packet);
            }
        }
    }
}
