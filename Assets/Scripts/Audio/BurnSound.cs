using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnSound : MonoBehaviour
{
    public SFX burnSFX;

    public void Play()
    {
        burnSFX.PlaySFX();
    }
}
