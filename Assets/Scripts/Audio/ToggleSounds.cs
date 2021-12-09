using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ToggleSounds : MonoBehaviour
{
    [SerializeField] private bool alwaysClickSFX = false;
    private Toggle toggle;

    private void Start()
    {
        toggle = gameObject.GetComponent<Toggle>();
        if (alwaysClickSFX) toggle.onValueChanged.AddListener((data) => OnClickAlways());
        else toggle.onValueChanged.AddListener((data) => OnClick());
    }

    public void OnClick()
    {
        if (!toggle.isOn) SFXLibrary.Instance.buttonClick.PlaySFX();
    }

    public void OnClickAlways()
    {
        SFXLibrary.Instance.buttonClick.PlaySFX();
    }
}
