using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EscMenu : MonoBehaviour
{
    public static EscMenu Instance { get; private set; }
    [SerializeField] private GameObject escMenu;
    [SerializeField] private GameObject escMenuButtons;
    [SerializeField] private GameObject volumeSliders;
    [SerializeField] private GameObject disconnectConfirmation;
    [SerializeField] private GameObject quitConfirmation;
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private float masterDefaultVolume;
    [SerializeField] private float musicDefaultVolume;
    [SerializeField] private float sfxDefaultVolume;
    private bool open = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", sfxDefaultVolume);
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
        escMenuButtons.SetActive(true);
        volumeSliders.SetActive(true);
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

    public void VolumeSlidersSetActive(bool value)
    {
        volumeSliders.SetActive(value);
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

    public void SetMasterVolume(float sliderValue)
    {
        masterMixer.SetFloat("masterVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float sliderValue)
    {
        masterMixer.SetFloat("mainMenuMusicVol", Mathf.Log10(sliderValue) * 20);
        masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(sliderValue) * 20);
        masterMixer.SetFloat("resultScreenMusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float sliderValue)
    {
        masterMixer.SetFloat("sfxVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }
}
