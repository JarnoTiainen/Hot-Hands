using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SoundtrackManager : MonoBehaviour
{
    public static SoundtrackManager Instance { get; private set; }
    private SoundtrackLibrary soundtrackLibrary;
    public static AudioMixer masterMixer;
    private static AudioMixerGroup mainMenuMusicGroup;
    private static AudioMixerGroup inGameMusicGroup;
    [SerializeField] private float masterDefaultVolume = 0.5f;
    [SerializeField] private float musicDefaultVolume = 0.5f;
    [SerializeField] private float mainMenuFadeIn = 1f;
    [SerializeField] private float mainMenuFadeOut = 1f;
    [SerializeField] private float inGameFadeIn = 1f;
    [SerializeField] private float inGameFadeOut = 1f;
    private int currentSceneIndex;

    private void Awake()
    {
        Instance = this;
        soundtrackLibrary = GetComponent<SoundtrackLibrary>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType<SoundtrackManager>().Length > 1)
        {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += ChangedActiveScene;

        masterMixer = Resources.Load("MasterMixer") as AudioMixer;
        mainMenuMusicGroup = masterMixer.FindMatchingGroups("MainMenuMusic")[0];
        inGameMusicGroup = masterMixer.FindMatchingGroups("InGameMusic")[0];
        StartMusic();
    }

    private void StartMusic()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (currentSceneIndex)
        {
            case 0:
                masterMixer.SetFloat("masterVol", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume)) * 20);
                masterMixer.SetFloat("mainMenuMusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume)) * 20);
                masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(0.0001f) * 20);
                soundtrackLibrary.mainMenu.PlaySoundtrack();
                break;
            case 1:
                masterMixer.SetFloat("masterVol", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume)) * 20);
                masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume)) * 20);
                masterMixer.SetFloat("mainMenuMusicVol", Mathf.Log10(0.0001f) * 20);
                soundtrackLibrary.inGame.PlaySoundtrack();
                break;
            default:
                masterMixer.SetFloat("masterVol", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume)) * 20);
                masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume)) * 20);
                masterMixer.SetFloat("mainMenuMusicVol", Mathf.Log10(0.0001f) * 20);
                soundtrackLibrary.inGame.PlaySoundtrack(); Debug.Log("Default soundtrack.");
                break;
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

    public static void PlaySoundtrack(SoundtrackClip soundtrack, bool waitToFinish = true, bool loop = true, bool useDefault = false, AudioSource audioSource = null)
    {
        if (audioSource == null) audioSource = Instance.defaultAudioSource;

        if (useDefault && audioSource == null)
        {
            Debug.LogError("You forgot to add a default audio source!");
            return;
        }

        if (!audioSource.isPlaying || !waitToFinish)
        {
            if (!useDefault)
            {
                GameObject soundtrackGameObject = new GameObject("SoundtrackPlayer");
                DontDestroyOnLoad(soundtrackGameObject);
                audioSource = soundtrackGameObject.AddComponent<AudioSource>();
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    case 0:
                        soundtrackGameObject.name = "MainMenuSoundtrackPlayer";
                        audioSource.outputAudioMixerGroup = mainMenuMusicGroup;
                        break;

                    case 1:
                        soundtrackGameObject.name = "InGameSoundtrackPlayer";
                        audioSource.outputAudioMixerGroup = inGameMusicGroup;
                        break;
                    default:
                        soundtrackGameObject.name = "InGameSoundtrackPlayer";
                        audioSource.outputAudioMixerGroup = inGameMusicGroup;
                        break;
                }
                soundtrackGameObject.transform.position = new Vector3(0, 0, 0);
                audioSource.clip = soundtrack.clip;
                audioSource.volume = soundtrack.volume;
                audioSource.pitch = soundtrack.pitch;
                audioSource.loop = loop;
                audioSource.Play();

                if (!loop)
                {
                    Destroy(soundtrackGameObject, audioSource.clip.length);
                }
            }
            else
            {
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    case 0:
                        audioSource.outputAudioMixerGroup = mainMenuMusicGroup;
                        break;
                    case 1:
                        audioSource.outputAudioMixerGroup = inGameMusicGroup;
                        break;
                    default:
                        audioSource.outputAudioMixerGroup = inGameMusicGroup;
                        break;
                }
                audioSource.clip = soundtrack.clip;
                audioSource.volume = soundtrack.volume;
                audioSource.pitch = soundtrack.pitch;
                audioSource.loop = loop;
                audioSource.Play();
            }
        }
    }

    public void CallStartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        StartCoroutine(FadeMixerGroup.StartFade(audioMixer, exposedParam, duration, targetVolume));
    }

    public void CallStopMusic(GameObject musicPlayer, float time)
    {
        StartCoroutine(FadeMixerGroup.StopMusic(musicPlayer, time));
    }

    public void CallDestroySoundtrack(GameObject soundtrack, float time)
    {
        StartCoroutine(FadeMixerGroup.DestroySoundtrack(soundtrack, time));
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        switch (currentSceneIndex)
        {
            case 0:
                StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "mainMenuMusicVol", mainMenuFadeOut, 0.0001f));
                StartCoroutine(FadeMixerGroup.DestroySoundtrack(GameObject.Find("MainMenuSoundtrackPlayer"), mainMenuFadeOut));
                break;
            case 1:
                StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "inGameMusicVol", inGameFadeOut, 0.0001f));
                StartCoroutine(FadeMixerGroup.DestroySoundtrack(GameObject.Find("InGameSoundtrackPlayer"), inGameFadeOut));
                break;
            default:
                StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "inGameMusicVol", inGameFadeOut, 0.0001f));
                StartCoroutine(FadeMixerGroup.DestroySoundtrack(GameObject.Find("InGameSoundtrackPlayer"), inGameFadeOut));
                break;
        }

        currentSceneIndex = next.buildIndex;
        switch (currentSceneIndex)
        {
            case 0:
                masterMixer.SetFloat("mainMenuMusicVol", Mathf.Log10(0.0001f) * 20);
                soundtrackLibrary.mainMenu.PlaySoundtrack();
                StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "mainMenuMusicVol", mainMenuFadeIn, PlayerPrefs.GetFloat("MusicVolume")));
                break;
            case 1:
                masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(0.0001f) * 20);
                soundtrackLibrary.inGame.PlaySoundtrack();
                StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "inGameMusicVol", inGameFadeIn, PlayerPrefs.GetFloat("MusicVolume")));
                break;
            default:
                masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(0.0001f) * 20);
                soundtrackLibrary.inGame.PlaySoundtrack();
                StartCoroutine(FadeMixerGroup.StartFade(masterMixer, "inGameMusicVol", inGameFadeIn, PlayerPrefs.GetFloat("MusicVolume")));
                break;
        }
    }

    [HorizontalGroup("AudioSource")]
    [ShowIf("@defaultAudioSource == null")]
    [GUIColor(1f, 0.5f, 0.5f, 1f)]
    [Button]
    private void AddAudioSource()
    {
        defaultAudioSource = gameObject.GetComponent<AudioSource>();

        if (defaultAudioSource == null)
            defaultAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public enum SoundtrackType
    {
        Menu,
        InGame,
    }
}