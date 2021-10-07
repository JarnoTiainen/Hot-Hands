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
    [SerializeField] private float musicFadeTime = 3f;

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
        AudioMixer masterMixer = Resources.Load("MasterMixer") as AudioMixer;
        GameObject soundtrackManager = GameObject.FindGameObjectWithTag("SoundtrackManager");
        soundtrackManager.GetComponent<SoundtrackManager>().CallStartFade(masterMixer, "inGameMusicVol", musicFadeTime, 0.0001f);
        soundtrackManager.GetComponent<SoundtrackManager>().CallDestroySoundtrack(GameObject.Find("InGameSoundtrackPlayer"), musicFadeTime);
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}
