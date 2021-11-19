using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public Animator dialogueAnimator;
    public Queue<string> sentences;

    private void Awake()
    {
        dialogueAnimator.SetBool("dialogueOn", true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
             dialogueAnimator.SetBool("dialogueOn", false);
        }

        if (Input.GetKeyDown(KeyCode.F)) {
             dialogueAnimator.SetBool("dialogueOn", true);
        }
    }
}
