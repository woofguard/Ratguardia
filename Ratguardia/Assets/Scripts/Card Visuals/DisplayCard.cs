using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class DisplayCard : MonoBehaviour
{   
    public Card card;

    public TextMeshPro cardName;
    public TextMeshPro effect;
    public TextMeshPro effectDetails;

    public Sprite cardBack;
    SpriteRenderer sprite;

    public void UpdateCardDisplay()
    {
        if(card.faceUp)
        {
            sprite.sprite = card.cardSprite;
        }
        else
        {
            sprite.sprite = cardBack;
        }
    }
}
