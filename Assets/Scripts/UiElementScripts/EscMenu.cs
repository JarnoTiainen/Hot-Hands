using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class EscMenu : MonoBehaviour
{
    public GameObject escMenuButtons;
    public GameObject settingsMenu;
    public GameObject disconnectConfirmation;
    public GameObject quitConfirmation;

    private void OnEnable()
    {
        escMenuButtons.SetActive(true);
        settingsMenu.SetActive(false);
        disconnectConfirmation.SetActive(false);
        quitConfirmation.SetActive(false);
    }

    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Return()
    {
        gameObject.SetActive(false);
    }

    public void EscMenuButtonsSetActive(bool value)
    {
        escMenuButtons.SetActive(value);
    }

    public void SettingsMenuSetActive(bool value)
    {
        settingsMenu.SetActive(value);
    }
    public void DisconnectConfirmationSetActive(bool value)
    {
        disconnectConfirmation.SetActive(value);
    }

    public void QuitConfirmationSetActive(bool value)
    {
        quitConfirmation.SetActive(value);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}
