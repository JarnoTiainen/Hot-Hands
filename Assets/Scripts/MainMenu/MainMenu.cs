using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    private GameObject canvas;
    public GameObject mainMenuButtons;
    public GameObject settingsMenu;
    public GameObject collectionMenu;
    public GameObject quitConfirmation;
    public GameObject loginScreen;
    public GameObject loadingScreen;
    [SerializeField] private GameObject popupNotification;
    [SerializeField] private TextMeshProUGUI popupNotificationText;
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
        Instance = this;
        canvas = GameObject.Find("Canvas");
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

    public void SettingsMenuSetActive(bool value) => settingsMenu.SetActive(value);

    public void QuitConfirmationSetActive(bool value) => quitConfirmation.SetActive(value);

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
        Debug.Log("Play");
        if(soloPlayEnabled)
        {
            LoadScene(1);
        }
    }

    public void GameFound(int scene)
    {
        LoadScene(scene);
        Debug.Log("Game Found");
    }

    private void LoadScene(int scene) => StartCoroutine(LoadAsynchronously(scene));

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

    public void CreatePopupNotification(string text) => StartCoroutine(ShowPopupNotification(text));

    IEnumerator ShowPopupNotification(string text)
    {
        float currentTime = 0;
        float duration = 2.5f;
        popupNotificationText.text = text;
        StartCoroutine(MovePopup(true));
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(MovePopup(false));
    }

    IEnumerator MovePopup(bool direction)
    {
        float duration = 0.3f;
        RectTransform popupRectTransform = popupNotification.GetComponent<RectTransform>();

        float popupYPosStart;
        float popupYPosEnd;
        if (direction)
        {
            popupYPosStart = popupRectTransform.anchoredPosition.y;
            popupYPosEnd = (popupNotification.GetComponent<RectTransform>().rect.height / 2);
        }
        else
        {
            popupYPosStart = popupRectTransform.anchoredPosition.y;
            popupYPosEnd = -(popupNotification.GetComponent<RectTransform>().rect.height / 2);
        }

        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newPos = Mathf.Lerp(popupYPosStart, popupYPosEnd, currentTime / duration);
            popupRectTransform.anchoredPosition = new Vector2(popupRectTransform.anchoredPosition.x, newPos);
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
