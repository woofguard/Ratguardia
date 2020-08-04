﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]

public class DisplayCard : MonoBehaviour
{   
    public Card card;

    public TextMeshPro cardName;
    public TextMeshPro atk;
    public TextMeshPro def;
    public TextMeshPro effect;
    public TextMeshPro effectDetails;

    public SpriteRenderer cardBack;
    public Sprite[] suitFrames;
    public SpriteRenderer sprite;
    public SpriteRenderer frame;
    public SpriteRenderer rubble;

    public Animator anim;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // set card display to current information
    public void SetCard(Card c)
    {
        card = c;
        cardName.text = card.cardName;
        atk.text = card.atk + "";
        def.text = card.def + "";
        sprite.sprite = card.cardSprite;

        switch (card.suit)
        {
            case Suit.Chalices:
                frame.sprite = suitFrames[0];
                break;
            case Suit.Swords:
                frame.sprite = suitFrames[1];
                break;
            case Suit.Wands:
                frame.sprite = suitFrames[2];
                break;
            case Suit.Rings:
                frame.sprite = suitFrames[3];
                break;
            default:
                break;
        }

        // set effect text
        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        if(card.faceUp)
        {
            cardBack.gameObject.SetActive(false);

            frame.gameObject.SetActive(true);
            cardName.gameObject.SetActive(true);
            atk.gameObject.SetActive(true);
            def.gameObject.SetActive(true);
            effect.gameObject.SetActive(true);
            effectDetails.gameObject.SetActive(true);

             rubble.gameObject.SetActive(card.rubble);

        }
        else
        {
            cardBack.gameObject.SetActive(true);

            //frame.gameObject.SetActive(false);
            //cardName.gameObject.SetActive(false);
            //atk.gameObject.SetActive(false);
            //def.gameObject.SetActive(false);
            //effect.gameObject.SetActive(false);
            //effectDetails.gameObject.SetActive(false);
            //rubble.gameObject.SetActive(false);
        }
    }
    
    public void HideCard()
    {
        sprite.enabled = false;
        frame.gameObject.SetActive(false);
        cardName.gameObject.SetActive(false);
        atk.gameObject.SetActive(false);
        def.gameObject.SetActive(false);
        effect.gameObject.SetActive(false);
        effectDetails.gameObject.SetActive(false);
    }

    public void UnhideCard()
    {
        sprite.enabled = true;
        frame.gameObject.SetActive(true);
        cardName.gameObject.SetActive(true);
        atk.gameObject.SetActive(true);
        def.gameObject.SetActive(true);
        effect.gameObject.SetActive(true);
        effectDetails.gameObject.SetActive(true);
    }

    public void UpdateStats()
    {
        atk.text = card.atk + "";
        def.text = card.def + "";
    }

    public void UpdateRubble()
    {
        rubble.gameObject.SetActive(card.rubble);
    }

    public void FlipUp()
    {
       // if (!card.faceUp) return;
        anim.Play("CardFlipUp");
    }

    public void FlipDown()
    {
        if (!card.faceUp) return;
        anim.Play("CardFlipDown");
    }
}
