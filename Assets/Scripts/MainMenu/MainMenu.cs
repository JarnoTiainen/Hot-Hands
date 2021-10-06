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

    public void Play(int scene)
    {
        AudioMixer masterMixer = Resources.Load("MasterMixer") as AudioMixer;
        GameObject soundtrackManager = GameObject.FindGameObjectWithTag("SoundtrackManager");
        soundtrackManager.GetComponent<SoundtrackManager>().CallStartFade(masterMixer, "mainMenuMusicVol", musicFadeTime, 0.0001f);
        soundtrackManager.GetComponent<SoundtrackManager>().CallDestroySoundtrack(GameObject.Find("MainMenuSoundtrackPlayer"), musicFadeTime);
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
