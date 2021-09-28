public class GameMessage
{
    
    public string action;
    public string type;
    public string obj;
    
    public GameMessage(string actionIn, string typeIn, string objIn)
    {
        action = actionIn;
        type = typeIn;
        obj = objIn;
    }
}
