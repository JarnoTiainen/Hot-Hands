using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardSFX : MonoBehaviour
{
    public SFX cardPlay1;
    public SFX cardPlay2;
    public SFX cardPlay3;

    public void Play()
    {
        //Debug.Log("Play sound");
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
                cardPlay1.PlaySFX();
                break;
        }
    }
}
