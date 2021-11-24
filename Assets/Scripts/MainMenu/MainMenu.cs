using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuButtons;
    public GameObject settingsMenu;
    public GameObject collectionMenu;
    public GameObject quitConfirmation;
    public GameObject loginScreen;
    public GameObject loadingScreen;
    public Slider progressSlider;
    public TextMeshProUGUI progressText;
    public bool soloPlayEnabled;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 mainCamMenuPos;
    [SerializeField] private Quaternion mainCamMenuRotation;
    [SerializeField] private Vector3 mainCamCollectionPos;
    [SerializeField] private Quaternion mainCamCollectionRotation;

    private void Start()
    {
        if (WebSocketService.Instance.isLoggedIn)
        {
            loginScreen.SetActive(false);
            Debug.Log("Login screen SetActive false");
        }
        else
        {
            loginScreen.SetActive(true);
            Debug.Log("isLoggedIn: " + WebSocketService.Instance.isLoggedIn);
            Debug.Log("Login screen SetActive true");
        }
    }

    public void MenuButtonsSetActive(bool value)
    {
        mainMenuButtons.SetActive(value);
        if (value) SetCameraMainMenu();
    }

    public void CollectionMenuSetActive(bool value)
    {
        if (collectionMenu.activeSelf == true) CollectionManager.Instance.MoveCollectionMenu(true);
        else collectionMenu.SetActive(value);
        if (value) SetCameraCollectionMenu();
    }

    public void SettingsMenuSetActive(bool value)
    {
        settingsMenu.SetActive(value);
    }

    public void QuitConfirmationSetActive(bool value)
    {
        quitConfirmation.SetActive(value);
    }

    public void SetCameraMainMenu()
    {
        mainCamera.transform.position = mainCamMenuPos;
        mainCamera.transform.rotation = mainCamMenuRotation;
    }

    public void SetCameraCollectionMenu()
    {
        mainCamera.transform.position = mainCamCollectionPos;
        mainCamera.transform.rotation = mainCamCollectionRotation;
    }

    public void Play()
    {
        WebSocketService.JoinGame(soloPlayEnabled);
        Debug.Log("play");
        if(soloPlayEnabled)
        {
            LoadScene(1);
        }
        
    }

    public void GameFound(int scene)
    {
        LoadScene(scene);
        Debug.Log("gameFound");
    }

    private void LoadScene(int scene)
    {
        StartCoroutine(LoadAsynchronously(scene));
    }

    IEnumerator LoadAsynchronously(int scene)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
        loadingScreen.SetActive(true);

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            progressSlider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }
    }

    public void OnTutorialButton()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}
