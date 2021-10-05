using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuButtons;
    public GameObject optionsMenu;
    public GameObject profileMenu;
    public GameObject quitConfirmation;
    public GameObject loadingScreen;

    public void MenuButtonsSetActive(bool value)
    {
        mainMenuButtons.SetActive(value);
    }
    public void ProfileMenuSetActive(bool value)
    {
        profileMenu.SetActive(value);
    }

    public void OptionsMenuSetActive(bool value)
    {
        optionsMenu.SetActive(value);
    }

    public void QuitConfirmationSetActive(bool value)
    {
        quitConfirmation.SetActive(value);
    }

    public void LaunchScene(int scene)
    {
        StartCoroutine(LoadingScreen(scene));
    }

    IEnumerator LoadingScreen(int scene)
    {
        loadingScreen.SetActive(true);
        Debug.Log("Launching scene " + scene);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}
