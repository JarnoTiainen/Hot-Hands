using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LoadChatMessage
{
    public List<string> rawMessages;
    public List<Message> parsedMessages;
}

[Serializable]
public class Message
{
    public string uuid;
    public string message;
    public string username;
}
