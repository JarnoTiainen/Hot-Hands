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
    public int handIndex;
    public int sender;
    public int boardIndex;


    public PlayCardMessage(int cardSource = 0, int handIndex = 0, int boardIndex = 0, int sender = 0)
    {
        this.cardSource = (CardSource)cardSource;
        this.handIndex = handIndex;
        this.sender = sender;
        this.boardIndex = boardIndex;
    }
}
