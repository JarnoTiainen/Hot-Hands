using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class MessageLinkScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string link = "No links found";
    public void FindLink()
    {
        string msg = messageText.text;
        if(msg.Contains("https://"))
        {
            int startIndex = msg.IndexOf("https://", 0);
            string linkStart = msg.Substring(startIndex, msg.Length - startIndex);
            int endIndex = linkStart.IndexOf(" ", 0);
            string linkString = linkStart.Substring(0, endIndex);

            string beforeLink = messageText.text.Substring(0, startIndex);
            string afterLink = messageText.text.Substring((startIndex + linkString.Length), msg.Length - (startIndex + linkString.Length));

            string hyperlinkedMsg = beforeLink + "<i><color=#009DFF><u><link=\"" + linkString + "\">" + linkString + "</link></u></color></i>" + afterLink;

            messageText.text = hyperlinkedMsg;
            link = linkString;
            return;
        }
        if (msg.Contains("http://"))
        {
            int startIndex = msg.IndexOf("http://", 0);
            string linkStart = msg.Substring(startIndex, msg.Length - startIndex);
            int endIndex = linkStart.IndexOf(" ", 0);
            string linkString = linkStart.Substring(0, endIndex);

            string beforeLink = messageText.text.Substring(0, startIndex);
            string afterLink = messageText.text.Substring((startIndex + linkString.Length), msg.Length - (startIndex + linkString.Length));

            string hyperlinkedMsg = beforeLink + "<i><color=#009DFF><u><link=\"" + linkString + "\">" + linkString + "</link></u></color></i>" + afterLink;

            messageText.text = hyperlinkedMsg;
            link = linkString;
            return;
        }
    }
}
