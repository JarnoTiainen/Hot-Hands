using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class SoundtrackManager : MonoBehaviour
{
    private static SoundtrackManager _instance;
    public static SoundtrackManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SoundtrackManager>();

            return _instance;
        }
    }

    [HorizontalGroup("AudioSource")]
    [SerializeField]
    private AudioSource defaultAudioSource;

    [TabGroup("Menu")]
    [AssetList(Path = "/Audio/Music/Menu", AutoPopulate = true)]
    public List<SoundtrackClip> menuSoundtrack;
    [TabGroup("InGame")]
    [AssetList(Path = "/Audio/Music/InGame", AutoPopulate = true)]
    public List<SoundtrackClip> inGameSoundtrack;
    [TabGroup("ResultScreen")]
    [AssetList(Path = "/Audio/Music/ResultScreen", AutoPopulate = true)]
    public List<SoundtrackClip> resultScreenSoundtrack;

    public static void PlaySoundtrack(SoundtrackClip soundtrack, bool waitToFinish = true, bool loop = true, AudioSource audioSource = null)
    {
        if (audioSource == null)
            audioSource = SoundtrackManager.instance.defaultAudioSource;

        if (audioSource == null)
        {
            Debug.LogError("You forgot to add a default audio source!");
            return;
        }

        if (!audioSource.isPlaying || !waitToFinish)
        {
            audioSource.clip = soundtrack.clip;
            audioSource.volume = soundtrack.volume;
            audioSource.pitch = soundtrack.pitch;
            audioSource.loop = loop;
            audioSource.Play();
        }
    }

    [HorizontalGroup("AudioSource")]
    [ShowIf("@defaultAudioSource == null")]
    [GUIColor(1f, 0.5f, 0.5f, 1f)]
    [Button]
    private void AddAudioSource()
    {
        defaultAudioSource = this.gameObject.GetComponent<AudioSource>();

        if (defaultAudioSource == null)
            defaultAudioSource = this.gameObject.AddComponent<AudioSource>();
    }

    public enum SoundtrackType
    {
        Menu,
        InGame,
        ResultScreen
    }
}





