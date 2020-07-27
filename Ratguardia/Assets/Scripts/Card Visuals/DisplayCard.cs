using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class DisplayCard : MonoBehaviour
{   
    public Card card;

    public TextMeshPro cardName;
    public TextMeshPro atk;
    public TextMeshPro def;
    public TextMeshPro effect;
    public TextMeshPro effectDetails;

    public Sprite cardBack;
    public Sprite[] suitFrames;
    [HideInInspector]
    public SpriteRenderer sprite;
    public SpriteRenderer frame;

    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // set card display to current information
    public void SetCard(Card c)
    {
        card = c;
        cardName.text = card.cardName;
        atk.text = card.atk + "";
        def.text = card.def + "";

        switch(card.suit)
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
            sprite.sprite = card.cardSprite;

            frame.gameObject.SetActive(true);
            cardName.gameObject.SetActive(true);
            atk.gameObject.SetActive(true);
            def.gameObject.SetActive(true);
            effect.gameObject.SetActive(true);
            effectDetails.gameObject.SetActive(true);
        }
        else
        {
            sprite.sprite = cardBack;

            frame.gameObject.SetActive(false);
            cardName.gameObject.SetActive(false);
            atk.gameObject.SetActive(false);
            def.gameObject.SetActive(false);
            effect.gameObject.SetActive(false);
            effectDetails.gameObject.SetActive(false);
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
}
