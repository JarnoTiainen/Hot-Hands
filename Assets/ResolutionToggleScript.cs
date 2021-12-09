using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionToggleScript : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = gameObject.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((data) => OnClick());
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveAllListeners();
    }

    private void OnClick()
    {
        if (toggle.isOn)
        {
            SettingsMenu.Instance.SetScreenResolution(gameObject.transform.GetSiblingIndex());
        }
    }
}
