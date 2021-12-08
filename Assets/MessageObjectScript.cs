using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class MessageObjectScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI messageText;
    private int maxMsgLimit = 200;
    private float marginMinusFactor = 2f;
    [SerializeField] private float timeMaxWidth = 90f;

    public void UpdateMessage(string time, string username, string message, bool admin)
    {
        timeText.text = "[" + time + "]";
        usernameText.text = username + ":";
        messageText.text = message;
        if (admin) usernameText.color = new Color32(255, 0, 100, 255);
        messageText.gameObject.GetComponent<MessageLinkScript>().FindLink();
    }

    public void FitMessageContent()
    {
        // Get's space left for the MessageText after username width
        float msgObjMaxWidth = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect.width;
        float usernameWidth = usernameText.preferredWidth;
        float spaceForMsg = msgObjMaxWidth - timeMaxWidth - usernameWidth;

        // Set's MessageText's width to the available space
        RectTransform msgRect = messageText.gameObject.GetComponent<RectTransform>();
        msgRect.sizeDelta = new Vector2(spaceForMsg, msgRect.rect.height);

        // Get's the heigth MessageText needs to show all lines of the message and sets it as the MessageObject's heigth
        float newMsgHeigth = messageText.preferredHeight;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(msgObjMaxWidth, newMsgHeigth);

        // Finally set's MessageText's heigth and positions it correctly
        msgRect.sizeDelta = new Vector2(spaceForMsg, newMsgHeigth);
        msgRect.anchoredPosition = new Vector2(-(spaceForMsg), 0);
    }

    // Leaving this here as a reminder of the suffering that is Unity UI
    private IEnumerator ResizeMessageText()
    {
        yield return null;
        if(messageText.textInfo.lineCount > 1)
        {
            int index = messageText.textInfo.lineInfo[0].lastCharacterIndex;
            string newText = messageText.text.Substring(index + System.Environment.NewLine.Length);

            float spaceAfterName = 130f / usernameText.GetComponent<RectTransform>().rect.width;
            spaceAfterName *= spaceAfterName;

            float newMargin = 10f + (0.5f * newText.Length) - ((marginMinusFactor * spaceAfterName) * ((float)newText.Length / (float)maxMsgLimit));
            messageText.margin = new Vector4(newMargin, 0, 0, 0);
        }
    }
}
