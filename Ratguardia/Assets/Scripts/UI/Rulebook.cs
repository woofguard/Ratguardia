using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rulebook : MonoBehaviour
{
    public GameObject controls;
    public GameObject overview;
    public GameObject cards;
    public GameObject stealing;

    //public Button controls;
    //public Button overview;
    //public Button cards;
    //public Button stealing;

    public void DisplayControls()
    {
        controls.SetActive(true);

        overview.SetActive(false);
        cards.SetActive(false);
        stealing.SetActive(false);
    }

    public void DisplayOverview()
    {
        overview.SetActive(true);

        controls.SetActive(false);
        cards.SetActive(false);
        stealing.SetActive(false);
    }

    public void DisplayCards()
    {
        cards.SetActive(true);

        controls.SetActive(false);
        overview.SetActive(false);
        stealing.SetActive(false);
    }

    public void DisplayStealing()
    {
        stealing.SetActive(true);

        controls.SetActive(false);
        overview.SetActive(false);
        cards.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
