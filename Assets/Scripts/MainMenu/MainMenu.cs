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
    public GameObject loadingScreen;
    public bool soloPlayEnabled;
    [SerializeField] private float musicFadeTime = 3f;

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
            StartCoroutine(LoadingScreen(1));
        }
    }
    public void GameFound(int scene)
    {
        StartCoroutine(LoadingScreen(scene));
        Debug.Log("gameFound");
    }

    IEnumerator LoadingScreen(int scene)
    {
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}
