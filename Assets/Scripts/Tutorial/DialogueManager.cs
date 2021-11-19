using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Animator dialogueAnimator;
    public Dialogue dialogue;
    public TextMeshProUGUI dialogueText;
    private Queue<string> sentences;

    private void Awake()
    {
        dialogueAnimator.SetBool("dialogueOn", true);
        sentences = new Queue<string>();
        //copy the dialogue sentences from the dialogue object
        foreach (string s in dialogue.sentences) {
            sentences.Enqueue(s);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
             dialogueAnimator.SetBool("dialogueOn", false);
        }

        if (Input.GetKeyDown(KeyCode.F)) {
             dialogueAnimator.SetBool("dialogueOn", true);
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            if(sentences.Count > 0) {
                StopAllCoroutines();
                string sentence = sentences.Dequeue();
                StartCoroutine(RollDialogue(sentence));
            } else {
                EndDialogue();
            }
            
        }

    }

    private void EndDialogue()
    {
        dialogueAnimator.SetBool("dialogueOn", false);
    }

    IEnumerator RollDialogue(string sentence)
    {
        dialogueText.text = "";
        foreach (char c in sentence.ToCharArray()) {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
