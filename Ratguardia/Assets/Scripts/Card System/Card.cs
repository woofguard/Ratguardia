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
    // struct to contain card effect function
    [System.Serializable]
    public struct Effect
    {
        public UnityEvent activateEvent;
        public UnityEvent deactivateEvent;
        public bool active;
    }

    public string cardName;
    public Suit suit;
    public int atk;
    public int def;

    public Sprite cardSprite;

    public bool faceUp;   // what side of the card is facing up visually, true = front, false = back 
    public bool revealed; // whether the opponents can see the card e.g. it is a rubble card
                          // a card can be faceUp visually but not revealed e.g. in your hand
    public Player owner;  // which player has the card
    public bool rubble;   // if the card is rubble

    public Effect effect;

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
        effect.activateEvent.Invoke();
    }

    public void DeactivateEffect()
    {
        effect.deactivateEvent.Invoke();
    }
}
