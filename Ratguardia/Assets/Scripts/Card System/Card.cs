using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Suit
{
    Chalices,
    Swords,
    Wands,
    Rings,
    Unknown // for when the card is face down
}

public class Card : MonoBehaviour
{
    public string cardName;
    public Suit suit;
    public int atk;
    public int def;
    private int baseAtk;
    private int baseDef;

    public Sprite cardSprite; // visualization stuff
    public DisplayCard visualCard;

    [HideInInspector] public bool faceUp;   // what side of the card is facing up visually, true = front, false = back 
    [HideInInspector] public bool revealed; // whether the opponents can see the card e.g. it is a rubble card
                                            // a card can be faceUp visually but not revealed e.g. in your hand
    [HideInInspector] public int owner;     // board array index of player who owns the card
    [HideInInspector] public bool rubble;   // if the card is rubble

    public UnityEvent effect;

    private void Start()
    {
        baseAtk = atk;
        baseDef = def;
        visualCard.SetCard(this);
    }

    // flips the card between face down/face up
    public void Flip()
    {
        if(faceUp)
        {
            faceUp = false;
            revealed = false;

            // play flip animation here or something
        }
        else
        {
            faceUp = true;

            // play flip animation here or something
        }
        visualCard.UpdateCardDisplay();
    }

    // overload to flip to specific state
    public void Flip(bool side)
    {
        // if card is already facing the side being set
        if(side == faceUp)
        {
            // do nothing
        }
        else
        {
            // sides dont match, flip card
            Flip();
        }
    }

    // reveals card to opponents/conceals if it is already revealed
    public void Reveal()
    {
        if(revealed)
        {
            revealed = false;

            if(faceUp)
            {
                Flip();
            }
        }
        else
        {
            revealed = true;
        }
    }

    // overload to set specific reveal state
    public void Reveal(bool side)
    {
        // if card state matches state being set
        if(side == revealed)
        {
            // do nothing
        }
        else
        {
            // sides dont match, reveal card
            Reveal();
        }
    }

    public void ActivateEffect()
    {
        effect.Invoke();
    }

    // reset the card's atk/def to base if it has been modified
    public void ResetStats()
    {
        if(atk != baseAtk || def != baseDef)
        {
            atk = baseAtk;
            def = baseDef;
        }
    }
}
