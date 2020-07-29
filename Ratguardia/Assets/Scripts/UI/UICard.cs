﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICard : MonoBehaviour
{
    // card illustration/frame
    public Image illustration;
    public Image frame;

    // card text elements
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI atk;
    public TextMeshProUGUI def;
    public TextMeshProUGUI effect;
    public TextMeshProUGUI effectDetails;

    // card owner information
    public TextMeshProUGUI ownerName;
    public Image ownerPortrait;

    // display a given card
    public void DisplayCard(Card card)
    {
        DisplayCard refCard = card.visualCard; // grab reference

        if (card.faceUp) illustration.sprite = refCard.sprite.sprite; // update sprites
        else illustration.sprite = card.cardSprite;
        frame.sprite = refCard.frame.sprite;

        cardName.text = refCard.cardName.text; // update text elements
        atk.text = refCard.atk.text;
        def.text = refCard.def.text;
        def.text = refCard.def.text;
        effectDetails.text = refCard.effectDetails.text;

        // commented out until we have character name & portrait info
        // ownerName.text = card.owner + ""; // set owner information
        // set ownerPortait
    }

    // use to show/hide owner display
    public void SetOwnerDisplay(bool disp)
    {
        ownerName.enabled = disp;
        ownerPortrait.enabled = disp;
    }

    // use to dim card
    public void SetDim(bool dim)
    {
        float col = dim ? 0.5f : 1.0f;

        illustration.color = new Color(col, col, col);
        frame.color = new Color(col, col, col);
    }
}
