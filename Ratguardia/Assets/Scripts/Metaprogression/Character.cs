using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string title;
    public Sprite portrait;

    public MatchDialogue dialogue;

    [System.Serializable]
    public struct MatchDialogue
    {
        public string[] intro;

        public string[] round1st;
        public string[] round2nd;
        public string[] roundlast;

        public string[] match1st;
        public string[] match2nd;
        public string[] matchlast;

        public string feed;
        public string spare;
        public string spared;
        public string death;
    }
}
