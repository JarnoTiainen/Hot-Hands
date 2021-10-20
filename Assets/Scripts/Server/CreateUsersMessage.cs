using UnityEngine;
using System;

[Serializable]
public class CreateUsersMessage
{
    public string userName;
    public string password;
    public string email;


    public CreateUsersMessage(string userName, string password = "", string email = "")
    {
        this.userName = userName;
        this.password = password;
        this.email = email;
    }
}
