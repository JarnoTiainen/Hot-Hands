using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnSFX : MonoBehaviour
{
    public SFX burnSFX;

    public void Play()
    {
        burnSFX.PlaySFX();
    }
}
