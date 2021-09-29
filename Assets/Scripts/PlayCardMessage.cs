public class PlayCardMessage
{
    public enum CardSource
    {
        Default,     //0
        Deck,        //1
        Hand,        //2
        DiscardPile, //3
    }
    public CardSource cardSource;
    public int index;
    public int sender;


    public PlayCardMessage(int cardSourceIn = 0, int indexIn = 0, int senderIn = 0)
    {
        cardSource = (CardSource)cardSourceIn;
        index = indexIn;
        sender = senderIn;
    }
}
