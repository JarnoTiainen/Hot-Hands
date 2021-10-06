using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SoundtrackManager : MonoBehaviour
{
    private static SoundtrackManager _instance;
    private static AudioMixer masterMixer;
    private static AudioMixerGroup mainMenuMusicGroup;
    private static AudioMixerGroup inGameMusicGroup;
    [SerializeField] private float masterDefaultVolume = 0.5f;
    [SerializeField] private float musicDefaultVolume = 0.5f;
    [SerializeField] private float musicSceneFade = 3f;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType<SoundtrackManager>().Length > 1)
        {
            Object.Destroy(gameObject);
        }

        SceneManager.sceneLoaded += SceneChange;

        masterMixer = Resources.Load("MasterMixer") as AudioMixer;
        mainMenuMusicGroup = masterMixer.FindMatchingGroups("MainMenuMusic")[0];
        inGameMusicGroup = masterMixer.FindMatchingGroups("InGameMusic")[0];

        StartMusic();
    }

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

    public static void PlaySoundtrack(SoundtrackClip soundtrack, bool waitToFinish = true, bool loop = true, bool useDefault = true, AudioSource audioSource = null)
    {
        if (audioSource == null)
            audioSource = SoundtrackManager.instance.defaultAudioSource;

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
                        soundtrackGameObject.name = "DefaultSoundtrackPlayer";
                        audioSource.outputAudioMixerGroup = mainMenuMusicGroup;
                        break;
                }
                soundtrackGameObject.transform.position = new Vector3(0, 0, 0);
                audioSource.maxDistance = 100f;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.dopplerLevel = 0f;
                audioSource.clip = soundtrack.clip;
                audioSource.volume = soundtrack.volume;
                audioSource.pitch = soundtrack.pitch;
                audioSource.loop = loop;
                audioSource.Play();

                if (!loop)
                {
                    Object.Destroy(soundtrackGameObject, audioSource.clip.length);
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
                        audioSource.outputAudioMixerGroup = mainMenuMusicGroup;
                        break;
                }
                audioSource.maxDistance = 100f;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.dopplerLevel = 0f;
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

 
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneChange;
    }
    private void SceneChange(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("SceneChange" + scene);
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                Debug.Log("SceneChange 0");

                gameObject.GetComponent<SoundtrackLibrary>().mainMenu.PlaySoundtrack();
                CallStartFade(masterMixer, "mainMenuMusicVol", musicSceneFade, PlayerPrefs.GetFloat("MusicVolume"));
                break;

            case 1:
                Debug.Log("SceneChange 1");

                gameObject.GetComponent<SoundtrackLibrary>().inGame.PlaySoundtrack();
                CallStartFade(masterMixer, "inGameMusicVol", musicSceneFade, PlayerPrefs.GetFloat("MusicVolume"));
                break;

            default:
                gameObject.GetComponent<SoundtrackLibrary>().defaultSoundtrack.PlaySoundtrack();
                CallStartFade(masterMixer, "mainMenuMusicVol", musicSceneFade, PlayerPrefs.GetFloat("MusicVolume"));
                break;
        }
    }

    private void StartMusic()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                masterMixer.SetFloat("masterVol", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume)) * 20);
                masterMixer.SetFloat("mainMenuMusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume)) * 20);
                masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(0.0001f) * 20);
                gameObject.GetComponent<SoundtrackLibrary>().mainMenu.PlaySoundtrack();
                break;

            case 1:
                masterMixer.SetFloat("masterVol", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", masterDefaultVolume)) * 20);
                masterMixer.SetFloat("inGameMusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", musicDefaultVolume)) * 20);
                masterMixer.SetFloat("mainMenuMusicVol", Mathf.Log10(0.0001f) * 20);
                gameObject.GetComponent<SoundtrackLibrary>().inGame.PlaySoundtrack();
                break;

            default:
                gameObject.GetComponent<SoundtrackLibrary>().defaultSoundtrack.PlaySoundtrack();
                break;
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





