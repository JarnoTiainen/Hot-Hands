public class PlayCard
{
    public enum CardSource
    {
        Default,     //0
        Deck,        //1
        Hand,        //2
        DiscardPile, //3
    }

    public string uuid;
    public string action;
    public CardSource cardSource;
    public int index;
    public int sender;

    public PlayCard(string actionIn, CardSource cardSourceIn = CardSource.Default, int indexIn = 0)
    {
        action = actionIn;
        cardSource = cardSourceIn;
        index = indexIn;
    }
}
