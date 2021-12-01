using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using UnityEngine.EventSystems;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance { get; private set; }
    public List<Message> messages;
    [SerializeField] private GameObject messageObject;
    [SerializeField] private GameObject chatContent;
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private TextMeshProUGUI characterCountText;
    private int charLimit;

    private void Awake()
    {
        Instance = this;
        charLimit = messageInput.characterLimit;
    }

    private void Start()
    {
        messageInput.onValueChanged.AddListener((call) => UpdateCharacterCount());
        UpdateCharacterCount();
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == messageInput.gameObject && Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }

    public void PopulateChat(List<Message> parsedMessages = null, bool newMessage = false)
    {
        foreach(Transform messageObject in chatContent.transform)
        {
            Destroy(messageObject.gameObject);
        }
        foreach(Message message in parsedMessages)
        {
            GameObject newMessageObject = Instantiate(messageObject);
            string rawDateString = message.uuid.Substring(16, 8);
            //System.DateTime dateTime = System.DateTime.ParseExact(rawDateString, "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
            //string timeString = dateTime.ToString();
            newMessageObject.GetComponent<MessageObjectScript>().UpdateMessage(rawDateString, message.username, message.message);
            newMessageObject.transform.SetParent(chatContent.transform, false);
        }
        foreach(Transform messageObj in chatContent.transform)
        {
            messageObj.GetComponent<MessageObjectScript>().FitMessageContent();
        }
        StartCoroutine(ForceUpdateLayout());
    }

    private IEnumerator ForceUpdateLayout()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());
    }

    private void SendMessage()
    {
        if (messageInput.text == "")
        {
            messageInput.ActivateInputField();
            messageInput.Select();
            return;
        }
        string message = messageInput.text;
        messageInput.text = "";
        WebSocketService.SendNewMessage(message);
        messageInput.ActivateInputField();
        messageInput.Select();
    }

    public void ToggleChat(bool open)
    {
        RectTransform chat = gameObject.GetComponent<RectTransform>();
        if (open)
        {
            chat.anchoredPosition = new Vector2(-400, 300);
        }
        else
        {
            chat.anchoredPosition = new Vector2(-400, -300);
        }
    }

    public void HideChat(bool hidden)
    {
        RectTransform chat = gameObject.GetComponent<RectTransform>();
        if (hidden)
        {
            chat.anchoredPosition = new Vector2(-400, -1000);
        }
        else
        {
            chat.anchoredPosition = new Vector2(-400, -300);
        }
    }

    private void UpdateCharacterCount()
    {
        int msgLength = messageInput.text.Length;
        characterCountText.text = msgLength + "/" + charLimit;
        if(msgLength >= charLimit)
        {
            characterCountText.color = Color.yellow;
        }
        else
        {
            characterCountText.color = Color.white;
        }
    }
}
