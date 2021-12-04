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
    private bool activePopup = false;

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
        searchingGame.SetActive(value);
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

    public void CreatePopupNotification(string text, PopupCorner corner)
    {
        if (activePopup) return;
        StartCoroutine(ShowPopupNotification(text, corner));
    }

    IEnumerator ShowPopupNotification(string text, PopupCorner corner)
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

        float currentTime = 0;
        popupNotification.SetActive(true);
        popupNotificationText.text = text;
        if (corner == PopupCorner.BottomLeft || corner == PopupCorner.BottomRight) StartCoroutine(MovePopup(true));
        else StartCoroutine(MovePopup(false));
        while (currentTime < popupDuration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        if (corner == PopupCorner.BottomLeft || corner == PopupCorner.BottomRight) StartCoroutine(MovePopup(false));
        else StartCoroutine(MovePopup(true));
        yield return new WaitForSeconds(popupMovementDuration);
        popupNotification.SetActive(false);
        activePopup = false;
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

    public enum PopupCorner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public void OnTutorialButton()
    {
        ChatManager.Instance.HideChat(true);
        SceneManager.LoadScene("TutorialScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}
