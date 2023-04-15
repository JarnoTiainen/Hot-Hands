using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    [SerializeField] private SplashArtAnimation splashArt;
    [SerializeField] private GameObject music;
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject collectionMenu;
    [SerializeField] private GameObject quitConfirmation;
    public GameObject loginScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject popupNotification;
    [SerializeField] private AdminControls adminControls;
    [SerializeField] private SearchingGamePopupScript searchingGame;
    [SerializeField] private TextMeshProUGUI popupNotificationText;
    [SerializeField] private float popupMovementDuration = 0.3f;
    public Slider progressSlider;
    public TextMeshProUGUI progressText;
    public bool soloPlayEnabled;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 mainCamMenuPos;
    [SerializeField] private Quaternion mainCamMenuRotation;
    [SerializeField] private Vector3 mainCamCollectionPos;
    [SerializeField] private Quaternion mainCamCollectionRotation;
    private Coroutine showPopupNotificationCo = null;
    private bool activePopup = false;
    private string previousPopup = null;

    private void Start()
    {
        Instance = this;
        if (WebSocketService.Instance.isLoggedIn)
        {
            Debug.Log("Logged in");
            music.SetActive(true);
            mainMenuButtons.SetActive(true);
            loginScreen.SetActive(false);
            ChatManager.Instance.HideChat(false);
            WebSocketService.GetDecks();
            WebSocketService.LoadChat();
            if (PlayerPrefs.GetInt("AdminFeatures", 0) == 1) adminControls.EnableAdminFeatures();
        }
        else
        {
            Debug.Log("Not logged in");
            Cursor.visible = false;
            splashArt.gameObject.SetActive(true);
            StartCoroutine(StartSoundsWithDelay(splashArt.AnimationDuration()));
        }
    }

    private IEnumerator StartSoundsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        music.SetActive(true);
    }

    public void MainMenuButtonsSetActive(bool value) => mainMenuButtons.SetActive(value);

    public void SuccessfulLogin(LoginMessage loginMessage)
    {
        PlayerPrefs.SetString("LoginName", loginMessage.username);
        if (loginMessage.admin)
        {
            adminControls.EnableAdminFeatures();
        }
    }

    public void CollectionMenuSetActive(bool value)
    {
        collectionMenu.SetActive(value);
        MainMenuButtonsSetActive(!value);
        ChatManager.Instance.HideChat(value);
        if (value)
        {
            CollectionManager.Instance.MoveCollectionMenu(true);
            SetCameraCollectionMenu();
        }
        else SetCameraMainMenu();
    }

    public void SettingsMenuSetActive(bool value)
    {
        settingsMenu.SetActive(value);
        ChatManager.Instance.HideChat(value);
        MainMenuButtonsSetActive(!value);
        if (value) SetCameraCollectionMenu();
        else SetCameraMainMenu();
    }

    public void QuitConfirmationSetActive(bool value)
    {
        quitConfirmation.SetActive(value);
        MainMenuButtonsSetActive(!value);
        ChatManager.Instance.HideChat(value);
        if (value) SetCameraCollectionMenu();
        else SetCameraMainMenu();
    }

    public void SearchingGamePanelSetActive(bool value)
    {
        searchingGame.gameObject.SetActive(value);
        MainMenuButtonsSetActive(!value);
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
        MainMenuButtonsSetActive(false);
        if(AdminControls.Instance != null)
        {
            soloPlayEnabled = AdminControls.Instance.SoloPlay();
        }
        WebSocketService.JoinGame(soloPlayEnabled);
        Debug.Log("Play");
        if (soloPlayEnabled)
        {
            GameManager.Instance.ResetPlayerStats();
            LoadScene(1);
        }
        else searchingGame.gameObject.SetActive(true);
    }

    public void GameFound(int scene)
    {
        LoadScene(scene);
        Debug.Log("Game Found");
    }

    private void LoadScene(int scene) => StartCoroutine(LoadAsynchronously(scene));

    IEnumerator LoadAsynchronously(int scene)
    {
        searchingGame.StopAllCoroutines();
        searchingGame.statusText.text = "Opponent found!";
        SFXLibrary.Instance.matchFound.PlaySFX();
        yield return new WaitForSeconds(1.5f);
        searchingGame.gameObject.SetActive(false);
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

    public void CreatePopupNotification(string text, PopupCorner corner, PopupTone tone, float popupDuration = 2.5f)
    {
        if (activePopup && (text == previousPopup)) return;
        previousPopup = text;
        if(showPopupNotificationCo != null) StopCoroutine(showPopupNotificationCo);
        showPopupNotificationCo = StartCoroutine(ShowPopupNotification(text, corner, tone, popupDuration));
    }

    private IEnumerator ShowPopupNotification(string text, PopupCorner corner, PopupTone tone, float popupDuration)
    {
        activePopup = true;
        RectTransform popupRectTransform = popupNotification.GetComponent<RectTransform>();
        switch (corner)
        {
            case PopupCorner.TopLeft:
                popupRectTransform.anchorMin = new Vector2(0, 1);
                popupRectTransform.anchorMax = new Vector2(0, 1);
                popupRectTransform.anchoredPosition = new Vector2(popupRectTransform.rect.width / 2, popupRectTransform.rect.height / 2);
                break;
            case PopupCorner.TopRight:
                popupRectTransform.anchorMin = new Vector2(1, 1);
                popupRectTransform.anchorMax = new Vector2(1, 1);
                popupRectTransform.anchoredPosition = new Vector2(-(popupRectTransform.rect.width / 2), popupRectTransform.rect.height / 2);
                break;
            case PopupCorner.BottomLeft:
                popupRectTransform.anchorMin = new Vector2(0, 0);
                popupRectTransform.anchorMax = new Vector2(0, 0);
                popupRectTransform.anchoredPosition = new Vector2(popupRectTransform.rect.width / 2, -(popupRectTransform.rect.height / 2));
                break;
            case PopupCorner.BottomRight:
                popupRectTransform.anchorMin = new Vector2(1, 0);
                popupRectTransform.anchorMax = new Vector2(1, 0);
                popupRectTransform.anchoredPosition = new Vector2(-(popupRectTransform.rect.width / 2), -(popupRectTransform.rect.height / 2));
                break;
            default:
                break;
        }
        switch (tone)
        {
            case PopupTone.Positive:
                SFXLibrary.Instance.notificationPositive.PlaySFX();
                break;
            case PopupTone.Negative:
                SFXLibrary.Instance.notificationNegative.PlaySFX();
                break;
        }

        popupNotification.SetActive(true);
        popupNotificationText.text = text;
        if (corner == PopupCorner.BottomLeft || corner == PopupCorner.BottomRight) StartCoroutine(MovePopup(true));
        else StartCoroutine(MovePopup(false));

        yield return new WaitForSeconds(popupDuration);

        if (corner == PopupCorner.BottomLeft || corner == PopupCorner.BottomRight) StartCoroutine(MovePopup(false));
        else StartCoroutine(MovePopup(true));
        yield return new WaitForSeconds(popupMovementDuration);

        popupNotification.SetActive(false);
        activePopup = false;
    }

    private IEnumerator MovePopup(bool direction)
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

    public enum PopupCorner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public enum PopupTone
    {
        Positive,
        Negative
    }

    public void OnTutorialButton()
    {
        GameManager.Instance.ResetPlayerStats();
        ChatManager.Instance.HideChat(true);
        SceneManager.LoadScene("TutorialScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
