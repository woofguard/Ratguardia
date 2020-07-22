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
    SpriteRenderer sprite;

    // set card display to current information
    public void SetCard(Card c)
    {
        card = c;
        cardName.text = card.cardName;
        atk.text = card.atk + "";
        def.text = card.def + "";
        // set effect text
    }

    public void UpdateCardDisplay()
    {
        if(card.faceUp)
        {
            sprite.sprite = card.cardSprite;

            cardName.gameObject.SetActive(true);
            atk.gameObject.SetActive(true);
            def.gameObject.SetActive(true);
            effect.gameObject.SetActive(true);
            effectDetails.gameObject.SetActive(true);
        }
        else
        {
            sprite.sprite = cardBack;

            cardName.gameObject.SetActive(false);
            atk.gameObject.SetActive(false);
            def.gameObject.SetActive(false);
            effect.gameObject.SetActive(false);
            effectDetails.gameObject.SetActive(false);
        }
    }
}
