using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    private static AudioMixer masterMixer;
    private static AudioMixerGroup sfxGroup;
    private static GameObject uiSFXGameObject;
    private static AudioSource uiSFXAudioSource;
    [SerializeField] private float sfxDefaultVolume = 0.5f;
    [SerializeField] private float playBuffer = 0.08f;
    private bool isPlaying = false;
    private AudioClip playingClip = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        masterMixer = Resources.Load("MasterMixer") as AudioMixer;
        sfxGroup = masterMixer.FindMatchingGroups("SFX")[0];
        masterMixer.SetFloat("sfxVol", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", sfxDefaultVolume)) * 20);
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

    private IEnumerator SFXBuffer(AudioClip audioClip)
    {
        isPlaying = true;
        playingClip = audioClip;
        yield return new WaitForSeconds(playBuffer);
        isPlaying = false;
    }

    public void PlaySFX(SFXClip sfx, bool waitToFinish = true, bool useDefault = true, AudioSource audioSource = null)
    {
        if (isPlaying && (playingClip.name == sfx.clip.name)) return;
        StopAllCoroutines();
        isPlaying = false;
        if (audioSource == null)
        {
            audioSource = defaultAudioSource;
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
                audioSource.outputAudioMixerGroup = sfxGroup;
                audioSource.clip = sfx.clip;
                audioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
                audioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
                audioSource.Play();

                Destroy(sfxGameObject, audioSource.clip.length);
            }
            else
            {
                audioSource.outputAudioMixerGroup = sfxGroup;
                audioSource.clip = sfx.clip;
                audioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
                audioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
                audioSource.Play();
            }
        }
        StartCoroutine(SFXBuffer(sfx.clip));
    }

    public void PlayUISFX(SFXClip sfx)
    {
        if (isPlaying && (playingClip.name == sfx.clip.name)) return;
        StopAllCoroutines();
        isPlaying = false;
        if (uiSFXGameObject == null)
        {
            uiSFXGameObject = new GameObject("UISounds");
            uiSFXAudioSource = uiSFXGameObject.AddComponent<AudioSource>();
            uiSFXAudioSource.outputAudioMixerGroup = sfxGroup;
        }
        uiSFXAudioSource.clip = sfx.clip;
        uiSFXAudioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
        uiSFXAudioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
        uiSFXAudioSource.Play();
        StartCoroutine(SFXBuffer(sfx.clip));
    }

    [HorizontalGroup("AudioSource")]
    [ShowIf("@defaultAudioSource == null")]
    [GUIColor(1f,0.5f,0.5f,1f)]
    [Button]
    private void AddAudioSource()
    {
        defaultAudioSource = gameObject.GetComponent<AudioSource>();

        if (defaultAudioSource == null) defaultAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public enum SFXType
    {
        UI,
        Ambient,
        Effects
    }
}





