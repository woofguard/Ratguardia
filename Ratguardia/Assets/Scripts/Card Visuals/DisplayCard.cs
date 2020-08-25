using System.Collections;
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

    private Vector3 pos;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        pos = transform.localPosition;
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
        }
        pos = transform.localPosition;
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
        GetComponent<BoxCollider2D>().enabled = false;
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
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void UpdateStats()
    {
        atk.text = card.atk + "";
        def.text = card.def + "";
    }

    public void UpdateRubble()
    {
        rubble.gameObject.SetActive(card.rubble);
        pos = transform.localPosition;
    }

    public void FlipUp()
    {
        // if (!card.faceUp) return;
        anim.StopPlayback();
        anim.Play("CardFlipUp");
        pos = transform.localPosition;
    }

    public void FlipDown()
    {
        //if (!card.faceUp) return;
        anim.StopPlayback();
        anim.Play("CardFlipDown");
        pos = transform.localPosition;
    }
	//plays send card to battle animation
	public void sendCard()
	{
        anim.StopPlayback();
		anim.Play("SendBattleCard");
	}
	//plays the retract card from battle animation 
	public void retractCard()
	{
        anim.StopPlayback();
        anim.Play("RetractBattleCard");
	}

    // highlights the card by moving up a bit
    public void Highlight()
    {
        //pos = card.transform.localPosition;
        //card.transform.localPosition = new Vector3(pos.x, pos.y + 0.2f, pos.z);
        anim.StopPlayback();
        anim.Play("HoverCard");
    }

    // moves the card back down
    public void UnHighlight()
    {
        //pos = card.transform.localPosition;
        //card.transform.localPosition = new Vector3(pos.x, pos.y - 0.2f, pos.z);
        anim.StopPlayback();
        anim.Play("UnHoverCard");
    }

}
