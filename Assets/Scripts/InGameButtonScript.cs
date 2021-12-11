using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        ResultQuitYes,
        TutorialBackToMenu
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
                EscMenu.Instance.QuitGame();
                break;
            case ButtonType.TutorialBackToMenu:

                WebSocketService.Instance.enabled = true;
                SceneManager.LoadScene(0);
                break;
            default:
                break;
        }
        SFXLibrary.Instance.buttonClick.PlaySFX();
    }

    public void OnHoverEnter()
    {
        ToggleButton(true);
        SFXLibrary.Instance.buttonHover.PlaySFX();
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
