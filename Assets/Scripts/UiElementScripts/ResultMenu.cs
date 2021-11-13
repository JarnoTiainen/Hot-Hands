using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class ResultMenu : MonoBehaviour
{
    public GameObject resultMenuButtons;
    public GameObject settingsMenu;
    public GameObject disconnectConfirmation;
    public GameObject quitConfirmation;

    public void Return()
    {
        gameObject.SetActive(false);
    }

    public void ResultMenuButtonsSetActive(bool value)
    {
        resultMenuButtons.SetActive(value);
    }

    public void SettingsMenuSetActive(bool value)
    {
        settingsMenu.SetActive(value);
    }
    public void ReturnConfirmationSetActive(bool value)
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
