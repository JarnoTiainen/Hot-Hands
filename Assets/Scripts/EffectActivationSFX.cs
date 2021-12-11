using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectActivationSFX : MonoBehaviour
{
    public SFX effectSFX;

    public void Play()
    {
        effectSFX.PlaySFX();
    }
}
