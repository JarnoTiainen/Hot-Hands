using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SFXLibrary : MonoBehaviour
{
    // Buttons
    public SFX buttonHover;
    public SFX buttonClick;

    // Card sounds
    public SFX cardDraw1;
    public SFX cardDraw2;
    public SFX cardDraw3;
    public SFX cardPlay1;
    public SFX cardPlay2;
    public SFX cardPlay3;
    public SFX burnCard;

    // Effects
    public SFX effectActivation;





    public void CardDraw()
    {
        int x = Random.Range(1, 4);
        switch (x)
        {
            case 1:
                cardDraw1.PlaySFX();
                break;
            case 2:
                cardDraw2.PlaySFX();
                break;
            case 3:
                cardDraw3.PlaySFX();
                break;
            default:
                break;
        }
    }

    public void CardPlay()
    {
        int x = Random.Range(1, 4);
        switch (x)
        {
            case 1:
                cardPlay1.PlaySFX();
                break;
            case 2:
                cardPlay2.PlaySFX();
                break;
            case 3:
                cardPlay3.PlaySFX();
                break;
            default:
                break;
        }
    }
}
