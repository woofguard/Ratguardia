using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RulebookPage : MonoBehaviour
{
    public GameObject[] textblocks;
    public Button prev;
    public Button next;

    int index = 0;

    private void Start()
    {
        TextMeshProUGUI[] tmp = transform.Find("Text Blocks").GetComponentsInChildren<TextMeshProUGUI>();
        textblocks = new GameObject[tmp.Length];
        for(int i = 0; i < textblocks.Length; i++)
        {
            textblocks[i] = tmp[i].gameObject;
            textblocks[i].SetActive(false);
        }
        textblocks[0].SetActive(true);
        prev.gameObject.SetActive(false);
        if(textblocks.Length == 1) next.gameObject.SetActive(false);
    }

    public void Next()
    {
        if (index >= textblocks.Length - 1) return;

        textblocks[index].SetActive(false);

        index++;
        textblocks[index].SetActive(true);

        if (index == textblocks.Length - 1)
        {
            next.gameObject.SetActive(false);
        } 
        if(index >= 1)
        {
            prev.gameObject.SetActive(true);
        } 
    }

    public void Prev()
    {
        if (index <= 0) return;

        textblocks[index].SetActive(false);
        index--;
        textblocks[index].SetActive(true);

        if (index == 0)
        {
            prev.gameObject.SetActive(false);
        }
        if (index == textblocks.Length - 2)
        {
            next.gameObject.SetActive(true);
        }
    }

}
