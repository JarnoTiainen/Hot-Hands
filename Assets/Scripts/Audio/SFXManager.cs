﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    private static SFXManager _instance;
    private static AudioMixer masterMixer;
    private static AudioMixerGroup sfxGroup;
    private static GameObject uiSFXGameObject;
    private static AudioSource uiSFXAudioSource;
    [SerializeField] private float sfxDefaultVolume = 0.5f;


    private void Start()
    {
        masterMixer = Resources.Load("MasterMixer") as AudioMixer;
        sfxGroup = masterMixer.FindMatchingGroups("SFX")[0];
        masterMixer.SetFloat("sfxVol", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", sfxDefaultVolume)) * 20);
    }

    public static SFXManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SFXManager>();

            return _instance;
        }
    }

    [HorizontalGroup("AudioSource")]
    [SerializeField]
    private AudioSource defaultAudioSource;

    [TabGroup("UI")]
    [AssetList(Path = "/Audio/SFX/UI", AutoPopulate = true)]
    public List<SFXClip> uiSFX;
    [TabGroup("Ambient")]
    [AssetList(Path = "/Audio/SFX/Ambient", AutoPopulate = true)]
    public List<SFXClip> ambientSFX;
    [TabGroup("Effects")]
    [AssetList(Path = "/Audio/SFX/Effects", AutoPopulate = true)]
    public List<SFXClip> effectsSFX;

    public static void PlaySFX(SFXClip sfx, bool waitToFinish = true, bool useDefault = true, AudioSource audioSource = null)
    {
        if (audioSource == null)
        {
            audioSource = SFXManager.instance.defaultAudioSource;
        }

        if (useDefault && audioSource == null)
        {
            Debug.LogError("You forgot to add a default audio source!");
            return;
        }

        if (!audioSource.isPlaying || !waitToFinish)
        {
            if (!useDefault)
            {
                GameObject sfxGameObject = new GameObject("SFX");
                sfxGameObject.transform.position = new Vector3(0, 0, 0);
                audioSource = sfxGameObject.AddComponent<AudioSource>();
                audioSource.maxDistance = 100f;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.dopplerLevel = 0f;
                audioSource.outputAudioMixerGroup = sfxGroup;
                audioSource.clip = sfx.clip;
                audioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
                audioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
                audioSource.Play();

                Object.Destroy(sfxGameObject, audioSource.clip.length);

            }
            else
            {
                audioSource.maxDistance = 100f;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.dopplerLevel = 0f;
                audioSource.outputAudioMixerGroup = sfxGroup;
                audioSource.clip = sfx.clip;
                audioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
                audioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
                audioSource.Play();
            }
        }
    }

    public static void PlayUISFX(SFXClip sfx)
    {
        if (uiSFXGameObject == null)
        {
            uiSFXGameObject = new GameObject("UISounds");
            uiSFXAudioSource = uiSFXGameObject.AddComponent<AudioSource>();
            uiSFXAudioSource.outputAudioMixerGroup = sfxGroup;
            Debug.Log("Created new UISounds GameObject");
        }
        uiSFXAudioSource.clip = sfx.clip;
        uiSFXAudioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
        uiSFXAudioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
        uiSFXAudioSource.Play();
    }

    [HorizontalGroup("AudioSource")]
    [ShowIf("@defaultAudioSource == null")]
    [GUIColor(1f,0.5f,0.5f,1f)]
    [Button]
    private void AddAudioSource()
    {
        defaultAudioSource = this.gameObject.GetComponent<AudioSource>();

        if (defaultAudioSource == null)
            defaultAudioSource = this.gameObject.AddComponent<AudioSource>();
    }

    public enum SFXType
    {
        UI,
        Ambient,
        Effects
    }
}





