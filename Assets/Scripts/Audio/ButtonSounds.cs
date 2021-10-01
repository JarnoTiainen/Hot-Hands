using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public SFX buttonHover;
    public SFX buttonClick;

    public void OnHover()
    {
        buttonHover.PlaySFX();
    }

    public void OnClick()
    {
        buttonClick.PlaySFX();
    }
}
