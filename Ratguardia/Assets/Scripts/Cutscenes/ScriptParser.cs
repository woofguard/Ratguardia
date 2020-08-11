using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScriptParser : MonoBehaviour
{

    // load file and return as Dialogue array
    public Dialogue[] LoadFile(string name)
    {
        TextAsset file = Resources.Load(name) as TextAsset;
        string[] lines = file.text.Split('\n');

        List<Dialogue> dialogues = new List<Dialogue>();
        for(int i = 2; i < lines.Length; i++) // skip first 2 lines
        {
            string line = lines[i];
            string[] segments = line.Split('	');

            Dialogue dialogue = new Dialogue();
            dialogue.name = segments[0];
            dialogue.line = segments[1];
            dialogue.effect = ""; // right now there are no effects so whatevs

            dialogues.Add(dialogue);
        }

        return dialogues.ToArray();

    }
}

public struct Dialogue
{
    public string name;
    public string line;
    public string effect;
}
