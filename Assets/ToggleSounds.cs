using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSounds : MonoBehaviour
{
    private static GameObject sfxLibrary;

    private void Start()
    {
        sfxLibrary = GameObject.Find("SFXLibrary");
    }

    public void OnHover()
    {
        if (!gameObject.GetComponent<Toggle>().isOn)
        {
            sfxLibrary.GetComponent<ButtonSFX>().buttonHover.PlaySFX();
        }
    }

    public void OnClick()
    {
        if (!gameObject.GetComponent<Toggle>().isOn)
        {
            sfxLibrary.GetComponent<ButtonSFX>().buttonClick.PlaySFX();
        }
    }
}
