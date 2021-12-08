using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminControls : MonoBehaviour
{
    public static AdminControls Instance { get; private set; }
    [SerializeField] private Toggle soloPlayToggle;

    private void Awake()
    {
        Instance = this;
    }

    public void EnableAdminFeatures()
    {
        LoginManager.Instance.userNameField.contentType = TMPro.TMP_InputField.ContentType.Standard;
    }

    public bool SoloPlay()
    {
        return soloPlayToggle.isOn;
    }
}
