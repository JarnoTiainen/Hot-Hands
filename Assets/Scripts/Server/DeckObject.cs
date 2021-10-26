
using System.Collections.Generic;

public class DeckObject
{
    public List<string> playerDeck = new List<string>();

    public DeckObject(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            playerDeck.Add(card.cardName);
        }
    }
}
