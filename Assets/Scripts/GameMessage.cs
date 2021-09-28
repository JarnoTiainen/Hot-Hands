[System.Serializable]
public class GameMessage
{
    public string messageType;
    public string obj;

    public GameMessage(string messageTypeIn, string objIn)
    {
        messageType = messageTypeIn;
        obj = objIn;
    }
}
