using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScriptParser))]
public class DialogueManager : MonoBehaviour
{

    ScriptParser sp;

    Dialogue[] currentDialogue;

    CutsceneCharacter lastChar;

    public CutsceneCursor cursor;

    bool skippedLine = false;

    // Start is called before the first frame update
    void Awake()
    {
        sp = GetComponent<ScriptParser>();
    }

    private void Start()
    {
        Debug.Log(StateManager.currentCutscene);
        PlayCutscene(StateManager.currentCutscene);
    }

    public void PlayCutscene(string name)
    {
        StateManager.main.inCutscene = true;
        currentDialogue = sp.LoadFile(name);
        StartCoroutine(RunDialogue());
    }

    public IEnumerator RunDialogue()
    {
       for(int i = 0; i < currentDialogue.Length; i++)
        {
            if (!(currentDialogue[i].name == null || currentDialogue[i].name == ""))
            {
                yield return StartCoroutine(RunLine(currentDialogue[i]));
                if (!skippedLine)
                {
                    yield return new WaitUntil(() => cursor.confirmPressed);
                    cursor.confirmPressed = false;
                }
                else skippedLine = false;
            }         
        }
        StateManager.main.AdvanceNarrative();
    }

    public IEnumerator RunLine(Dialogue line)
    {
        bool skipLine = false;

        

        Debug.Log(line.name);
        CutsceneCharacter character = transform.Find(line.name).GetComponent<CutsceneCharacter>();

        if (line.effects.Length >= 1)
        {
            foreach (string effect in line.effects)
            {
                switch (effect)
                {
                    case "ifAlive":
                        bool containsChar = false;
                        for(int i = 0; i < StateManager.main.combatants.Length; i++)
                        {
                            string c = StateManager.main.combatants[i];
                            if (c == line.name && i != StateManager.charDeath) containsChar = true;
                        }
                        skipLine = !containsChar;
                        break;
                    case "ifFirst":
                        if (StateManager.lastVictor > -1 && !(StateManager.main.combatants[StateManager.lastVictor] == line.name)) skipLine = true;
                        break;
                    case "ifNotFirst":
                        if (StateManager.lastVictor > -1 && StateManager.main.combatants[StateManager.lastVictor] == line.name) skipLine = true;
                        break;
                    case "updatePos":
                        character.transform.position = GetUpdatedPosition(line.name);
                        break;
                    case "hideLast":
                        if (StateManager.charDeath > -1) transform.Find(StateManager.main.combatants[StateManager.charDeath]).gameObject.SetActive(false);
                        break;
                    default:
                        break;
                }
                if (skipLine) break;
            }
        }
        if (skipLine)
        {
            skippedLine = true;
            yield break;
        }

        character.gameObject.SetActive(true);
        character.OpenTextbox();
        if (lastChar != null && lastChar != character) lastChar.CloseTextbox();

        string dialogueLine = "";
        char[] charArray = line.line.ToCharArray();

        for(int i = 0; i < line.line.Length; i++)
        {
            if(cursor.confirmPressed)
            {
                dialogueLine = line.line;
                character.SetLine(dialogueLine);
                cursor.confirmPressed = false;
                break;
            }

            dialogueLine += charArray[i];
            character.SetLine(dialogueLine);
            yield return new WaitForSeconds(0.05f);
        }
        Debug.Log(dialogueLine);
        lastChar = character;
        yield return null;
    }

    Vector3 GetUpdatedPosition(string name)
    {
        
        int ind = -1;
        for(int i = 0; i < StateManager.main.combatants.Length; i++)
        {
            if (StateManager.main.combatants[i] == name || i == StateManager.charDeath)
            {
                ind = i;
                break;
            }
        }
        Debug.Log(name + " " + ind);
        if (ind == -1) return new Vector3(0f, 0f, 0f);

        switch(ind)
        {
            case 1:
                return transform.Find("The Peasant").position;
            case 2:
                return transform.Find("The Knight").position;
            case 3:
                return transform.Find("The Cavalier").position;
            default:
                break;
        }

        return new Vector3(0f, 0f, 0f);
    }
}
