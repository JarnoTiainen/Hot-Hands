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


    public void CreateNewAccount()
    {
        WebSocketService.CreateNewAccount(userNameField.text, passwordField.text, emailField.text);
    }

    public void Login()
    {
        WebSocketService.Login(userNameField.text, passwordField.text);
        WebSocketService.GetDecks();
    }


    [Button]
    public void CloseLoginScreen()
    {
        GameObject.Find("Canvas").GetComponent<MainMenu>().MenuButtonsSetActive(true);
        gameObject.SetActive(false);
    }
}
