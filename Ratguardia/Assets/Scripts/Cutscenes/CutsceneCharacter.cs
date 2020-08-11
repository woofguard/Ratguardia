using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CutsceneCharacter : MonoBehaviour
{
    public GameObject textbox;
    public TextMeshProUGUI line;


    public void OpenTextbox()
    {
        textbox.SetActive(true);
        line.text = "";
    }

    public void CloseTextbox()
    {
        textbox.SetActive(false);
        line.text = "";
    }

    public void SetLine(string text)
    {
        line.text = text;
    }
}
