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
        WebSocketService.Login(userNameField.text, passwordField.text);
    }

    public void OpenSignup()
    {
        loginButton.SetActive(false);
        openSignUpButton.SetActive(false);
        signUpButton.SetActive(true);
        backButton.SetActive(true);
        emailField.gameObject.SetActive(true);
    }

    public void GoBack()
    {
        emailField.gameObject.SetActive(false);
        signUpButton.SetActive(false);
        backButton.SetActive(false);
        loginButton.SetActive(true);
        openSignUpButton.SetActive(true);
    }

    [Button]
    public void CloseLoginScreen()
    {
        mainMenuButtons.SetActive(true);
        gameObject.SetActive(false);
    }
}
