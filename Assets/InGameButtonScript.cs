using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InGameButtonScript : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private ButtonType buttonType;

    public enum ButtonType
    {
        Return,
        Settings,
        SettingsBack,
        Disconnect,
        DisconnectNo,
        DisconnectYes,
        EscQuit,
        ResultQuit,
        ResultQuitNo,
        QuitNo,
        QuitYes
    }

    public void OnClickElement()
    {
        switch (buttonType)
        {
            case ButtonType.Return:
                EscMenu.Instance.Return();
                break;
            case ButtonType.Settings:
                EscMenu.Instance.EscMenuButtonsSetActive(false);
                EscMenu.Instance.SettingsMenuSetActive(true);
                break;
            case ButtonType.SettingsBack:
                EscMenu.Instance.EscMenuButtonsSetActive(true);
                EscMenu.Instance.SettingsMenuSetActive(false);
                break;
            case ButtonType.Disconnect:
                EscMenu.Instance.EscMenuButtonsSetActive(false);
                EscMenu.Instance.DisconnectConfirmationSetActive(true);
                break;
            case ButtonType.DisconnectNo:
                EscMenu.Instance.EscMenuButtonsSetActive(true);
                EscMenu.Instance.DisconnectConfirmationSetActive(false);
                break;
            case ButtonType.DisconnectYes:
                EscMenu.Instance.ReturnToMenu();
                break;
            case ButtonType.EscQuit:
                EscMenu.Instance.EscMenuButtonsSetActive(false);
                EscMenu.Instance.QuitConfirmationSetActive(true);
                break;
            case ButtonType.ResultQuit:
                MatchResultScript.Instance.ResultScreenButtonsSetActive(false);
                MatchResultScript.Instance.ResultQuitConfirmationButtonsSetActive(true);
                break;
            case ButtonType.ResultQuitNo:
                MatchResultScript.Instance.ResultScreenButtonsSetActive(true);
                MatchResultScript.Instance.ResultQuitConfirmationButtonsSetActive(false);
                break;
            case ButtonType.QuitNo:
                EscMenu.Instance.EscMenuButtonsSetActive(true);
                EscMenu.Instance.QuitConfirmationSetActive(false);
                break;
            case ButtonType.QuitYes:
                EscMenu.Instance.QuitGame();
                break;
            default:
                break;
        }
    }

    public void OnHoverEnter()
    {
        ToggleButton(true);
    }

    public void OnHoverExit()
    {
        ToggleButton(false);
    }

    private void Awake()
    {
        meshRenderer.material = material;
    }

    [Button]
    private void ToggleButton(bool state)
    {
        if (state)
        {
            meshRenderer.material.SetInt("_ButtonOn", 1);
        }
        else
        {
            meshRenderer.material.SetInt("_ButtonOn", 0);
        }
    }
}
