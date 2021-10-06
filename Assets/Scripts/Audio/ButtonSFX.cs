using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public SFX buttonHover;
    public SFX buttonClick;
    public SFX playClick;

    public void OnHover()
    {
        buttonHover.PlaySFX();
    }

    public void OnClick()
    {
        buttonClick.PlaySFX();
    }
}
