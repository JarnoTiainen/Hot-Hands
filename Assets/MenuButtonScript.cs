using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MenuButtonScript : MonoBehaviour, IOnHoverEnterElement, IOnHoverExitElement, IOnClickDownUIElement
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private ButtonType buttonType;
    
    private enum ButtonType
    {
        Play,
        Tutorial,
        Collection,
        Settings,
        Quit
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
                MainMenu.Instance.SetCameraCollectionMenu();
                MainMenu.Instance.MainMenuButtonsSetActive(false);
                break;
            case ButtonType.Settings:
                MainMenu.Instance.SettingsMenuSetActive(true);
                MainMenu.Instance.SetCameraCollectionMenu();
                MainMenu.Instance.MainMenuButtonsSetActive(false);
                break;
            case ButtonType.Quit:
                MainMenu.Instance.QuitConfirmationSetActive(true);
                MainMenu.Instance.SetCameraCollectionMenu();
                MainMenu.Instance.MainMenuButtonsSetActive(false);
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
