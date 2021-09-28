using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class VolumeSettings : MonoBehaviour
{

    public AudioMixer masterMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    [SerializeField] private float masterDefaultVolume;
    [SerializeField] private float musicDefaultVolume;
    [SerializeField] private float sfxDefaultVolume;
    private bool inCombat = false;

    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", sfxDefaultVolume);
    }

    public void SetMasterVolume(float sliderValue)
    {
        masterMixer.SetFloat("masterVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }
    public void SetMusicVolume(float sliderValue)
    {
        if (inCombat)
        {
            masterMixer.SetFloat("combatMusicVol", Mathf.Log10(sliderValue) * 20);
        }
        else
        {
            masterMixer.SetFloat("musicVol", Mathf.Log10(sliderValue) * 20);
            masterMixer.SetFloat("combatMusicVol", Mathf.Log10(0.0001f) * 20);
        }
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }
    public void SetSFXVolume(float sliderValue)
    {
        masterMixer.SetFloat("sfxVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }
}