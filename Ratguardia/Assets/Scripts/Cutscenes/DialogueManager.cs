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
            if (!(currentDialogue[i].name == null || currentDialogue[i].name == "")) yield return StartCoroutine(RunLine(currentDialogue[i]));
            yield return new WaitUntil(() => cursor.confirmPressed);
            cursor.confirmPressed = false;
        }
        StateManager.main.AdvanceNarrative();
    }

    public IEnumerator RunLine(Dialogue line)
    {
        Debug.Log(line.name);
        CutsceneCharacter character = transform.Find(line.name).GetComponent<CutsceneCharacter>();
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
}
