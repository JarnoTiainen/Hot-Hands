using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolumeOnSceneLoad : MonoBehaviour
{
    private AudioMixer masterMixer;
    [SerializeField] private float masterDefaultVolume = 0.5f;
    [SerializeField] private float musicDefaultVolume = 0.5f;
    [SerializeField] private float sfxDefaultVolume = 0.5f;

    void Start()
    {
        masterMixer = Resources.Load("MasterMixer") as AudioMixer;
        masterMixer.SetFloat("masterVol", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume)) * 20);
        masterMixer.SetFloat("mainMenuMusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume)) * 20);
        masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume)) * 20);
        masterMixer.SetFloat("sfxVol", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", sfxDefaultVolume)) * 20);
    }
}
