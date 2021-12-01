using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance { get; private set; }
    [SerializeField] private TMP_InputField userNameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TextMeshProUGUI characterCounterText;
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject openSignUpButton;
    [SerializeField] private GameObject signUpButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject mainMenuButtons;

    private void Start()
    {
        Instance = this;
        userNameField.ActivateInputField();
        userNameField.Select();
    }

    public void CreateNewAccount()
    {
        WebSocketService.CreateNewAccount(userNameField.text, passwordField.text, emailField.text);
    }

    public void Login()
    {
        Debug.Log("loging");
        WebSocketService.Login(userNameField.text, passwordField.text);
    }

    public void OpenSignup()
    {
        loginButton.SetActive(false);
        openSignUpButton.SetActive(false);
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
        characterCounterText.gameObject.SetActive(false);
        userNameField.onValueChanged.RemoveAllListeners();
    }



    private void UpdateCharacterCount()
    {
        int msgLength = userNameField.text.Length;
        characterCounterText.text = msgLength + "/20";
        if (msgLength >= 20)
        {
            characterCounterText.color = Color.yellow;
        }
        else
        {
            characterCounterText.color = Color.white;
        }
    }

    [Button]
    public void CloseLoginScreen()
    {
        mainMenuButtons.SetActive(true);
        gameObject.SetActive(false);
    }
}
