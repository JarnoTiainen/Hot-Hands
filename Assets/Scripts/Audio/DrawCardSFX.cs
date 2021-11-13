using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardSFX : MonoBehaviour
{
    public SFX cardDraw1;
    public SFX cardDraw2;
    public SFX cardDraw3;

    public void Play()
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
                cardDraw1.PlaySFX();
                break;
        }
    }
}
