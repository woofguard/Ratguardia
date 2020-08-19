using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

    public static BackgroundManager main;

    public Background[] backgrounds;

    private SpriteRenderer sr;

    private void Awake()
    {
        // persistent singleton
        if (main == null)
        {
            main = this;
            sr = GetComponent<SpriteRenderer>();
            SetBackground(StateManager.background);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBackground(string name)
    {
        if (name == "") return;

        Sprite bg = null;
        for(int i = 0; i < backgrounds.Length; i++)
        {
            if(backgrounds[i].name == name)
            {
                bg = backgrounds[i].sprite;
                break;
            }
        }

        if (bg != null) sr.sprite = bg;
        else Debug.Log(name + " is not a valid background name");
    }

    [System.Serializable]
    public struct Background
    {
        public string name;
        public Sprite sprite;
    }
}
