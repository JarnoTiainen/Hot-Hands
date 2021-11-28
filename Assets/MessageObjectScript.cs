using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageObjectScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI messageText;

    public void UpdateMessage(string time, string username, string message)
    {
        timeText.text = "[" + time + "]";
        usernameText.text = username + ":";
        messageText.text = message;
    }
}
