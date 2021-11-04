using System;

[Serializable]
public class RemoveCardMessage
{
    public enum CardSource
    {
        Default,     //0
        Deck,        //1
        Hand,        //2
        DiscardPile, //3
    }
    public bool removal;
    public int source;
    public int player;
    public int handIndex;
    public int fieldIndex;
    public string seed;
}
