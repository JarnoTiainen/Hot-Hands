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
    [SerializeField] private TextMeshProUGUI nameCharacterCounterText;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI passwordText;
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject openSignUpButton;
    [SerializeField] private GameObject signUpButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private Toggle rememberMeToggle;
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private int nameMinLength = 3;
    [SerializeField] private int pwMinLength = 3;
    private bool nameAboveMinLength = false;
    private bool pwAboveMinLength = false;
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
        SaveUserCredentials();
        Debug.Log("Saved credentials");
    }

    public void CreateNewAccount()
    {
        if(nameAboveMinLength && pwAboveMinLength) WebSocketService.CreateNewAccount(userNameField.text, passwordField.text, emailField.text = "");
        else
        {
            if (!nameAboveMinLength)
            {
                MainMenu.Instance.CreatePopupNotification("Name must be over " + nameMinLength + " characters long!", MainMenu.PopupCorner.BottomLeft, MainMenu.PopupTone.Negative);
            }
            else if (!pwAboveMinLength)
            {
                MainMenu.Instance.CreatePopupNotification("Password must be over " + pwMinLength + " characters long!", MainMenu.PopupCorner.BottomLeft, MainMenu.PopupTone.Negative);
            }
        }
    }

    public void Login()
    {
        if (userNameField.text.Length == 0 || passwordField.text.Length == 0) return;
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
        //emailField.gameObject.SetActive(true);
        nameCharacterCounterText.gameObject.SetActive(true);
        userNameField.onValueChanged.AddListener((call) => UpdateNameCharacterCount());
        passwordField.onValueChanged.AddListener((call) => UpdatePasswordCharacterCount());
        UpdateNameCharacterCount();
        UpdatePasswordCharacterCount();
    }

    public void CloseSignup()
    {
        //emailField.gameObject.SetActive(false);
        signUpButton.SetActive(false);
        backButton.SetActive(false);
        loginButton.SetActive(true);
        openSignUpButton.SetActive(true);
        rememberMeToggle.gameObject.SetActive(true);
        nameCharacterCounterText.gameObject.SetActive(false);
        userNameField.onValueChanged.RemoveAllListeners();
        passwordField.onValueChanged.RemoveAllListeners();
        usernameText.color = Color.white;
        passwordText.color = Color.white;
    }

    private void UpdateNameCharacterCount()
    {
        int nameLength = userNameField.text.Length;
        nameCharacterCounterText.text = nameLength + "/" + charLimit;
        if (nameLength >= charLimit)
        {
            nameCharacterCounterText.color = Color.yellow;
            usernameText.color = Color.white;
            nameAboveMinLength = true;
        }
        else if(nameLength < nameMinLength)
        {
            nameCharacterCounterText.color = Color.red;
            usernameText.color = Color.red;
            nameAboveMinLength = false;
        }
        else
        {
            nameCharacterCounterText.color = Color.white;
            usernameText.color = Color.white;
            nameAboveMinLength = true;
        }
    }

    private void UpdatePasswordCharacterCount()
    {
        int pwLength = passwordField.text.Length;
        if (pwLength < pwMinLength)
        {
            passwordText.color = Color.red;
            pwAboveMinLength = false;
        }
        else
        {
            passwordText.color = Color.white;
            pwAboveMinLength = true;
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

    private void SaveUserCredentials()
    {
        PlayerPrefs.SetString("Username", userNameField.text);
        PlayerPrefs.SetString("Password", passwordField.text);
        PlayerPrefs.Save();
    }

    [Button]
    public void CloseLoginScreen()
    {
        mainMenuButtons.SetActive(true);
        gameObject.SetActive(false);
    }
}
