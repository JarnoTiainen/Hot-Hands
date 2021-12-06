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
        OpenSurrender,
        SurrenderNo,
        SurrenderYes,
        EscQuit,
        QuitNo,
        QuitYes,
        BackToMenu,
        ResultQuit,
        ResultQuitNo,
        ResultQuitYes
    }

    public void OnClickElement()
    {
        switch (buttonType)
        {
            case ButtonType.Return:
                EscMenu.Instance.Return();
                break;
            case ButtonType.OpenSurrender:
                EscMenu.Instance.EscMenuButtonsSetActive(false);
                EscMenu.Instance.VolumeSlidersSetActive(false);
                EscMenu.Instance.SurrenderConfirmationSetActive(true);
                break;
            case ButtonType.SurrenderNo:
                EscMenu.Instance.EscMenuButtonsSetActive(true);
                EscMenu.Instance.VolumeSlidersSetActive(true);
                EscMenu.Instance.SurrenderConfirmationSetActive(false);
                break;
            case ButtonType.SurrenderYes:
                WebSocketService.Surrender();
                EscMenu.Instance.Return();
                break;
            case ButtonType.EscQuit:
                EscMenu.Instance.EscMenuButtonsSetActive(false);
                EscMenu.Instance.VolumeSlidersSetActive(false);
                EscMenu.Instance.QuitConfirmationSetActive(true);
                break;
            case ButtonType.BackToMenu:
                EscMenu.Instance.ReturnToMenu();
                break;
            case ButtonType.ResultQuit:
                MatchResultScript.Instance.ResultScreenButtonsSetActive(false);
                MatchResultScript.Instance.ResultQuitConfirmationButtonsSetActive(true);
                break;
            case ButtonType.ResultQuitNo:
                MatchResultScript.Instance.ResultScreenButtonsSetActive(true);
                MatchResultScript.Instance.ResultQuitConfirmationButtonsSetActive(false);
                break;
            case ButtonType.ResultQuitYes:
                EscMenu.Instance.QuitGame();
                break;
            case ButtonType.QuitNo:
                EscMenu.Instance.EscMenuButtonsSetActive(true);
                EscMenu.Instance.VolumeSlidersSetActive(true);
                EscMenu.Instance.QuitConfirmationSetActive(false);
                break;
            case ButtonType.QuitYes:
                WebSocketService.Surrender();
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
