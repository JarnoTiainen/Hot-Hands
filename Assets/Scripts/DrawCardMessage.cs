using System;
[Serializable]
public class DrawCardMessage
{
    public int player;
    public string card;

    public DrawCardMessage(int playerIn, string cardIn = null)
    {
        player = playerIn;
        card = cardIn;
    }
}
