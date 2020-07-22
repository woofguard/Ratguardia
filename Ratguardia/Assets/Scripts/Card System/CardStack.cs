using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    [SerializeField]  // does not work, i guess stack cant be serialized......... big F
    public Stack<Card> stack;

    // which side the stack is facing (face down for deck, face up for rubble pile)
    public bool faceUp;
    public bool revealed;

    private void Awake()
    {
        stack = new Stack<Card>();
    }

    // draws the top card from stack
    public Card Pop()
    {
        // play animation??

        return stack.Pop(); 
    }

    // places card on top of the stack
    public void Push(Card c)
    {
        stack.Push(c);

        // set the card to the same side that the stack is
        c.Flip(faceUp);
        c.Reveal(revealed);
    }

    // shuffles the stack
    public void Shuffle()
    {
        // convert to array, unfortunately
        Card[] array = stack.ToArray();
        System.Random r = new System.Random();

        // run through the stack
        for(int i = 0; i < array.Length; i++)
        {
            // pick a random card between current card and the end
            int j = r.Next(i, array.Length);

            // exchange the two cards
            Card swap = array[i];
            array[i] = array[j];
            array[j] = swap;
        }

        // convert back to stack
        stack = new Stack<Card>(array);

        Debug.Log("deck shuffled");
    }
}
