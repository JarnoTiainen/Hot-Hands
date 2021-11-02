
using System.Collections.Generic;

public class DeckObject
{
    public List<string> playerDeck = new List<string>();
    public int deckIndex;

    public DeckObject(List<Card> cards, int deckIndex)
    {
        this.deckIndex = deckIndex;
        foreach (Card card in cards)
        {
            playerDeck.Add(card.cardName);
        }
    }
}
