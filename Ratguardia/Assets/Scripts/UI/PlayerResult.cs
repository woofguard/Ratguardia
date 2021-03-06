﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerResult : MonoBehaviour
{
    public TextMeshProUGUI roundPoints;
    public TextMeshProUGUI totalPoints;

    public Image portrait;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI line;

    public UICard[] cards;

    public bool showCards = false;

    public void ToggleCards()
    {
        showCards = !showCards;
        Animator anim = GetComponent<Animator>();

        if (showCards) anim.Play("ShowCards");
        else anim.Play("HideCards");
    }
}
