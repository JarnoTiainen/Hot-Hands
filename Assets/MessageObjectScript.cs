using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class MessageObjectScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI messageText;
    private int maxMsgLimit = 200;
    [SerializeField] private float marginMinusFactor = 2f;


    public void UpdateMessage(string time, string username, string message)
    {
        timeText.text = "[" + time + "]";
        usernameText.text = username + ":";
        messageText.text = message;
        StartCoroutine(ResizeMessageText());
    }

    [Button]
    IEnumerator ResizeMessageText()
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
