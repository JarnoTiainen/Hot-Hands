using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuButtons;
    public GameObject settingsMenu;
    public GameObject collectionMenu;
    public GameObject quitConfirmation;
    public GameObject loginScreen;
    public bool soloPlayEnabled;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 mainCamMenuPos;
    [SerializeField] private Vector3 mainCamCollectionPos;
    [SerializeField] private Quaternion mainCamMenuRotation;
    [SerializeField] private Quaternion mainCamCollectionRotation;



    private void Start()
    {
        if (WebSocketService.Instance.isLoggedIn == false)
        {
            loginScreen.SetActive(true);
        }
        else
        {
            loginScreen.SetActive(false);
        }
    }

    public void MenuButtonsSetActive(bool value)
    {
        mainMenuButtons.SetActive(value);
        if (value) SetCameraMainMenu();
    }

    public void CollectionMenuSetActive(bool value)
    {
        collectionMenu.SetActive(value);
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
        WebSocketService.JoinGame();
        Debug.Log("play");
        if(soloPlayEnabled)
        {
            SceneManager.LoadScene(1);
        }
    }
    public void GameFound(int scene)
    {
        SceneManager.LoadScene(1);
        Debug.Log("gameFound");
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
