using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Suit
{
    Chalices,
    Swords,
    Wands,
    Rings
}

public class Card : MonoBehaviour
{
    // struct to contain card effect function
    [System.Serializable]
    public struct Effect
    {
        public UnityEvent _event;
        public bool active;
    }

    private string _cardName;
    private Suit _suit;
    private int _atk;
    private int _def;

    // should only be able to read properties if the you can see it (face-up)
    public string cardName
    {
        get
        {
            if(faceUp)
            {
                return _cardName;
            }
            else
            {
                return null;
            }
        }
        set
        {

        }
    }

    public bool faceUp;   // what side of the card is facing up visually, true = front, false = back 
    public bool revealed; // whether the opponents can see the card e.g. it is a rubble card
                          // a card can be faceUp visually but not revealed e.g. in your hand
    

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

    public void ActivateEffect()
    {

    }

    public void DeactivateEffect()
    {

    }
}
