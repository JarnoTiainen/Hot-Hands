using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSounds : MonoBehaviour
{
    public void OnHover()
    {
        if (!gameObject.GetComponent<Toggle>().isOn)
        {
            SFXLibrary.Instance.buttonHover.PlaySFX();
        }
    }

    public void OnClick()
    {
        if (!gameObject.GetComponent<Toggle>().isOn)
        {
            SFXLibrary.Instance.buttonClick.PlaySFX();
        }
    }

    public void OnClickAlways()
    {
        SFXLibrary.Instance.buttonClick.PlaySFX();
    }
}
