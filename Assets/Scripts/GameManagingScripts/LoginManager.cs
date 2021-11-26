using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField userNameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private GameObject mainMenuButtons;

    public void CreateNewAccount()
    {
        WebSocketService.CreateNewAccount(userNameField.text, passwordField.text, emailField.text);
    }

    public void Login()
    {
        WebSocketService.Login(userNameField.text, passwordField.text);
    }

    [Button]
    public void CloseLoginScreen()
    {
        mainMenuButtons.SetActive(true);
        gameObject.SetActive(false);
    }
}
