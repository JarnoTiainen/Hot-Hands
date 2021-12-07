using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SFXLibrary : MonoBehaviour
{
    public static SFXLibrary Instance { get; private set; }

    // Main Menu
    public SFX buttonHover;
    public SFX buttonClick;
    public SFX notificationPositive;
    public SFX notificationNegative;
    public SFX matchFound;

    // Card sounds
    public SFX cardDraw1;
    public SFX cardDraw2;
    public SFX cardDraw3;
    public SFX cardPlay1;
    public SFX cardPlay2;
    public SFX cardPlay3;

    // Effects
    public SFX burnCard;
    public SFX cardDestroyed;
    public SFX effectActivation;
    public SFX hit;
    public SFX cardBuff;
    public SFX cardDebuff;
    public SFX cardHeal;

    // Other
    public SFX victory;
    public SFX defeat;
    public SFX countdown;
    public SFX spellChainCountdown;

    private void Awake()
    {
        Instance = this;
    }

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
