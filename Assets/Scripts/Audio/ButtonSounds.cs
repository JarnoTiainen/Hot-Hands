using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    private static GameObject sfxLibrary;

    private void Start()
    {
        sfxLibrary = GameObject.Find("SFXLibrary");
    }

    public void OnHover()
    {
        sfxLibrary.GetComponent<ButtonSFX>().buttonHover.PlaySFX();
    }

    public void OnClick()
    {
        sfxLibrary.GetComponent<ButtonSFX>().buttonClick.PlaySFX();
    }
}
