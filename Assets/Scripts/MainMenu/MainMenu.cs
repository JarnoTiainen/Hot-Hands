using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject collectionMenu;
    [SerializeField] private GameObject quitConfirmation;
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject popupNotification;
    [SerializeField] private GameObject searchingGame;
    [SerializeField] private TextMeshProUGUI popupNotificationText;
    [SerializeField] private float popupDuration = 2.5f;
    [SerializeField] private float popupMovementDuration = 0.3f;
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
        if (WebSocketService.Instance.isLoggedIn)
        {
            loginScreen.SetActive(false);
            Debug.Log("Login screen SetActive false");
            ChatManager.Instance.HideChat(false);
        }
        else
        {
            mainMenuButtons.SetActive(false);
            loginScreen.SetActive(true);
            Debug.Log("isLoggedIn: " + WebSocketService.Instance.isLoggedIn);
            Debug.Log("Login screen SetActive true");
        }
    }

    public void MainMenuButtonsSetActive(bool value) => mainMenuButtons.SetActive(value);

    public void CollectionMenuSetActive(bool value)
    {
        if (collectionMenu.activeSelf == true) CollectionManager.Instance.MoveCollectionMenu(true);
        else collectionMenu.SetActive(value);
        if (value) SetCameraCollectionMenu();
    }

    public void SettingsMenuSetActive(bool value) => settingsMenu.SetActive(value);

    public void QuitConfirmationSetActive(bool value) => quitConfirmation.SetActive(value);

    public void SearchingGamePanelSetActive(bool value) => searchingGame.SetActive(value);

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
        if (soloPlayEnabled) LoadScene(1);
        else searchingGame.SetActive(true);
    }

    public void GameFound(int scene)
    {
        searchingGame.SetActive(false);
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
        popupNotification.SetActive(true);
        popupNotificationText.text = text;
        StartCoroutine(MovePopup(true));
        while (currentTime < popupDuration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(MovePopup(false));
        yield return new WaitForSeconds(popupMovementDuration);
        popupNotification.SetActive(false);
    }

    IEnumerator MovePopup(bool direction)
    {
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
        while (currentTime < popupMovementDuration)
        {
            currentTime += Time.deltaTime;
            float newPos = Mathf.Lerp(popupYPosStart, popupYPosEnd, currentTime / popupMovementDuration);
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
