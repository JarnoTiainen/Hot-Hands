using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoginKeyboardCommands : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signupButton;
    public int inputIndex;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            inputIndex--;
            if (emailField.gameObject.activeSelf)
            {
                if (inputIndex < 0) inputIndex = 2;
            }
            else if (inputIndex < 0) inputIndex = 1;
            SelectInputField();
        }
        else if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            inputIndex++;
            if (emailField.gameObject.activeSelf)
            {
                if (inputIndex > 2) inputIndex = 0;
            }
            else if (inputIndex > 1) inputIndex = 0;
            SelectInputField();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterPressed();
        }
    }

    private void EnterPressed()
    {
        if (emailField.gameObject.activeSelf) signupButton.onClick.Invoke();
        else loginButton.onClick.Invoke();
    }

    private void SelectInputField()
    {
        switch (inputIndex)
        {
            case 0: usernameField.Select();
                break;
            case 1: passwordField.Select();
                break;
            case 2: emailField.Select();
                break;
        }
    }

    public void UsernameFieldSelected() => inputIndex = 0;
    public void PasswordFieldSelected() => inputIndex = 1;
    public void EmailFieldSelected() => inputIndex = 2;
}
