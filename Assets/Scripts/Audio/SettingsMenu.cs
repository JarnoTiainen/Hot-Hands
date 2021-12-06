using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    [SerializeField] private float masterDefaultVolume;
    [SerializeField] private float musicDefaultVolume;
    [SerializeField] private float sfxDefaultVolume;
    public Toggle[] resolutionToggles;
    public Toggle fullscreenToggle;
    public int[] screenWidths;
    private int activeScreenResIndex;
    [SerializeField] private int defaultScreenResIndex = 0;

    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", sfxDefaultVolume);

        activeScreenResIndex = PlayerPrefs.GetInt("ScreenResIndex", defaultScreenResIndex);
        bool isFullscreen = (PlayerPrefs.GetInt("Fullscreen") == 1) ? true : false;

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
                resolutionToggles[i].isOn = i == activeScreenResIndex;
        }
        fullscreenToggle.isOn = isFullscreen;
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

    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("ScreenResIndex", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }
        if (isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }
        PlayerPrefs.SetInt("Fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
        //mainCam.GetComponent<ForceAspectRatio>().FuckYouNico();
        //canvasCam.GetComponent<ForceAspectRatio>().FuckYouNico();
    }
}