using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class EscMenu : MonoBehaviour
{
    public static EscMenu Instance { get; private set; }
    [SerializeField] private GameObject escMenu;
    [SerializeField] private GameObject escMenuButtons;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject disconnectConfirmation;
    [SerializeField] private GameObject quitConfirmation;
    private bool open = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (open)
            {
                CloseEscMenu();
                open = false;
            }
            else
            {
                escMenu.SetActive(true);
                open = true;
            }
        }
    }

    private void CloseEscMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        settingsMenu.SetActive(false);
        disconnectConfirmation.SetActive(false);
        quitConfirmation.SetActive(false);
        escMenu.SetActive(false);
    }

    public void Return()
    {
        escMenu.SetActive(false);
        open = false;
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
