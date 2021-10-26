using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuButtons;
    public GameObject settingsMenu;
    public GameObject profileMenu;
    public GameObject quitConfirmation;
    public GameObject loginScreen;
    public bool soloPlayEnabled;
    [SerializeField] private float musicFadeTime = 3f;


    private void Start()
    {
        if (WebSocketService.Instance.isLoggedIn == false)
        {
            loginScreen.SetActive(true);
        }
        else
        {
            loginScreen.SetActive(false);
        }
    }

    public void MenuButtonsSetActive(bool value)
    {
        mainMenuButtons.SetActive(value);
    }
    public void ProfileMenuSetActive(bool value)
    {
        profileMenu.SetActive(value);
    }

    public void SettingsMenuSetActive(bool value)
    {
        settingsMenu.SetActive(value);
    }

    public void QuitConfirmationSetActive(bool value)
    {
        quitConfirmation.SetActive(value);
    }

    public void Play()
    {
        WebSocketService.JoinGame();
        Debug.Log("play");
        if(soloPlayEnabled)
        {
            SceneManager.LoadScene(1);
        }
    }
    public void GameFound(int scene)
    {
        SceneManager.LoadScene(1);
        Debug.Log("gameFound");
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}
