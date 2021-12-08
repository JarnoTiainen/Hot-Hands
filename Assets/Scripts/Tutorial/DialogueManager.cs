using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class DialogueManager : MonoBehaviour
{
    public Animator dialogueAnimator;
    public Animator mascotAnimator;
    public Dialogue dialogue;
    public TextMeshProUGUI dialogueText;
    [BoxGroup("Highlighters")]
    public List<HighLightController> highLightControllers;
    private TutorialManager tutorialManager;
    private Queue<string> sentences;
    public bool startBool = false;
    private Coroutine roll;
    private string sentence;


    private void Awake()
    {
        sentence = "";
        roll = null;
        //dialogueAnimator.SetBool("dialogueOn", true);
        sentences = new Queue<string>();

        //copy the dialogue sentences from the dialogue object
        foreach (string s in dialogue.sentences) {
            sentences.Enqueue(s);
        }

        tutorialManager = TutorialManager.tutorialManagerInstance;

        StartCoroutine(StartDialogue());
    }

    private void Update()
    {
        if ((int)tutorialManager.GetState() % 2 == 0) {
            //if (dialogueAnimator.GetBool("dialogueOn") == false) {
            //    StartCoroutine(StartDialogue());
            //    Debug.Log("Starting dialogue ");
            //    startBool = true;
            //}

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || startBool) {

                startBool = false;
                if (sentences.Count > 0) {
                    sentence = sentences.Dequeue();
                    if (sentence == "") {
                        
                        EndDialogue();
                        tutorialManager.NextTutorialState();
                    } else {
                        StopAllCoroutines();
                        StartCoroutine(RollDialogue(sentence));
                    }


                } else {
                    EndDialogue();
                }
            }
        }


    }

    public void DialogueTrigger()
    {
        StartCoroutine(StartDialogue());
    }

    private void EndDialogue()
    {
        //Debug.Log("Ending dialogue");
        dialogueAnimator.SetBool("dialogueOn", false);
        mascotAnimator.SetBool("dialogueOn", false);
    }

    public void SkipNextDialogue()
    {
        sentence = sentences.Dequeue();
        while(sentence != "") {
            sentence = sentences.Dequeue();
        }
        //StartCoroutine(StartDialogue());
    }

    IEnumerator RollDialogue(string sentence)
    {
        dialogueText.text = "";
        foreach (char c in sentence.ToCharArray()) {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator StartDialogue()
    {
        //Debug.Log("Startin dialogue");
        dialogueText.text = "";
        
        dialogueAnimator.SetBool("dialogueOn", true);
        mascotAnimator.SetBool("dialogueOn", true);
        yield return new WaitForSecondsRealtime(0.5f);
        startBool = true;
        
    }

    private void ToggleTime()
    {
        if(Time.timeScale == 0) {
            Time.timeScale = 1f;
        } else {
            Time.timeScale = 0;
        }
    }
}
