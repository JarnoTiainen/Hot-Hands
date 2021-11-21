using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class DialogueManager : MonoBehaviour
{
    public Animator dialogueAnimator;
    public Dialogue dialogue;
    public TextMeshProUGUI dialogueText;
    [BoxGroup("Highlighters")]
    public List<HighLightController> highLightControllers;
    private Queue<string> sentences;
    private int effectCounter = 0;
    private bool startBool = false;


    private void Awake()
    {
        //dialogueAnimator.SetBool("dialogueOn", true);
        sentences = new Queue<string>();

        //copy the dialogue sentences from the dialogue object
        foreach (string s in dialogue.sentences) {
            sentences.Enqueue(s);
        }

        StartCoroutine(StartDialogue());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) {
            dialogueAnimator.SetBool("dialogueOn", false);
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            dialogueAnimator.SetBool("dialogueOn", true);
        }

        if (Input.GetKeyDown(KeyCode.Space) || startBool) {
            startBool = false;
            if(dialogueAnimator.GetBool("dialogueOn") == false) {
                dialogueAnimator.SetBool("dialogueOn", true);
            }

            if(sentences.Count > 0) {
                StopAllCoroutines();
                string sentence = sentences.Dequeue();
                if (sentence == "") {
                    dialogueAnimator.SetBool("dialogueOn", false);
                    DialogueTrigger();
                } else {
                    StartCoroutine(RollDialogue(sentence));
                }
                
            } else {
                EndDialogue();
            }
        }

    }

    public void DialogueTrigger()
    {
        highLightControllers[effectCounter].ToggleHighlightAnimation();
        effectCounter++;
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

    IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(1f);
        startBool = true;
    }
}
