using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance { get; private set; }
    public TMP_InputField userNameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TextMeshProUGUI characterCounterText;
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject openSignUpButton;
    [SerializeField] private GameObject signUpButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private Toggle rememberMeToggle;
    [SerializeField] private GameObject mainMenuButtons;
    private int charLimit;

    private void Start()
    {
        Instance = this;
        userNameField.ActivateInputField();
        userNameField.Select();
        charLimit = userNameField.characterLimit;
        rememberMeToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("RememberMe", 0));

        if (rememberMeToggle.isOn) GetUserCredentials();
    }

    private void OnDisable()
    {
        ToggleRememberMe(rememberMeToggle.isOn);
        SaveUserCredentials(rememberMeToggle.isOn);
        Debug.Log("Saved credentials");
    }

    public void CreateNewAccount()
    {
        WebSocketService.CreateNewAccount(userNameField.text, passwordField.text, emailField.text);
    }

    public void Login()
    {
        Debug.Log("login");
        WebSocketService.Login(userNameField.text, passwordField.text);
    }

    public void OpenSignup()
    {
        loginButton.SetActive(false);
        openSignUpButton.SetActive(false);
        rememberMeToggle.gameObject.SetActive(false);
        signUpButton.SetActive(true);
        backButton.SetActive(true);
        emailField.gameObject.SetActive(true);
        characterCounterText.gameObject.SetActive(true);
        userNameField.onValueChanged.AddListener((call) => UpdateCharacterCount());
        UpdateCharacterCount();
    }

    public void GoBack()
    {
        emailField.gameObject.SetActive(false);
        signUpButton.SetActive(false);
        backButton.SetActive(false);
        loginButton.SetActive(true);
        openSignUpButton.SetActive(true);
        rememberMeToggle.gameObject.SetActive(true);
        characterCounterText.gameObject.SetActive(false);
        userNameField.onValueChanged.RemoveAllListeners();
    }

    private void UpdateCharacterCount()
    {
        int msgLength = userNameField.text.Length;
        characterCounterText.text = msgLength + "/" + charLimit;
        if (msgLength >= charLimit)
        {
            characterCounterText.color = Color.yellow;
        }
        else
        {
            characterCounterText.color = Color.white;
        }
    }

    private void ToggleRememberMe(bool value)
    {
        PlayerPrefs.SetInt("RememberMe", System.Convert.ToInt32(value));
        PlayerPrefs.Save();
    }

    private void GetUserCredentials()
    {
        userNameField.text = PlayerPrefs.GetString("Username", "");
        passwordField.text = PlayerPrefs.GetString("Password", "");
        Debug.Log("Get credentials");
    }

    private void SaveUserCredentials(bool rememberMe)
    {
        if (rememberMe)
        {
            PlayerPrefs.SetString("Username", userNameField.text);
            PlayerPrefs.SetString("Password", passwordField.text);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetString("Username", "");
            PlayerPrefs.SetString("Password", "");
            PlayerPrefs.Save();
        }
    }

    [Button]
    public void CloseLoginScreen()
    {
        mainMenuButtons.SetActive(true);
        gameObject.SetActive(false);
    }
}
