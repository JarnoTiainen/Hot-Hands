using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MenuButtonScript : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private ButtonType buttonType;

    public enum ButtonType
    {
        Play,
        Tutorial,
        Collection,
        Settings,
        Quit,
        Login,
        OpenSignup,
        Signup,
        LoginBack,
        SettingsBack,
        QuitYes,
        QuitNo,
        CancelSearch
    }

    public void OnClickElement()
    {
        switch(buttonType)
        {
            case ButtonType.Play:
                MainMenu.Instance.Play();
                break;
            case ButtonType.Tutorial:
                MainMenu.Instance.OnTutorialButton();
                break;
            case ButtonType.Collection:
                MainMenu.Instance.CollectionMenuSetActive(true);
                break;
            case ButtonType.Settings:
                MainMenu.Instance.SettingsMenuSetActive(true);
                break;
            case ButtonType.Quit:
                MainMenu.Instance.QuitConfirmationSetActive(true);
                break;
            case ButtonType.Login:
                LoginManager.Instance.Login();
                break;
            case ButtonType.OpenSignup:
                LoginManager.Instance.OpenSignup();
                break;
            case ButtonType.Signup:
                LoginManager.Instance.CreateNewAccount();
                break;
            case ButtonType.LoginBack:
                LoginManager.Instance.GoBack();
                break;
            case ButtonType.SettingsBack:
                MainMenu.Instance.SettingsMenuSetActive(false);
                break;
            case ButtonType.QuitYes:
                MainMenu.Instance.QuitGame();
                break;
            case ButtonType.QuitNo:
                MainMenu.Instance.QuitConfirmationSetActive(false);
                MainMenu.Instance.SetCameraMainMenu();
                MainMenu.Instance.MainMenuButtonsSetActive(true);
                ChatManager.Instance.HideChat(false);
                break;
            case ButtonType.CancelSearch:
                Debug.Log("Canceling search");
                WebSocketService.StopSearch();
                MainMenu.Instance.SearchingGamePanelSetActive(false);
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
        if(state)
        {
            meshRenderer.material.SetInt("_ButtonOn", 1);
        }
        else
        {
            meshRenderer.material.SetInt("_ButtonOn", 0);
        }
    }
}
