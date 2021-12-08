using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMessage : MonoBehaviour
{
    public string username;
    public bool admin;

    public LoginMessage(string username, bool admin)
    {
        this.username = username;
        this.admin = admin;
    }
}
