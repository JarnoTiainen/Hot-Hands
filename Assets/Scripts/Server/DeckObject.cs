
using System.Collections.Generic;

public class DeckObject
{
    public List<string> playerDeck = new List<string>();
    public int deckIndex;
    public string deckName;

    public DeckObject(List<Card> cards, int deckIndex, string deckName)
    {
        this.deckIndex = deckIndex;
        this.deckName = deckName;
        foreach (Card card in cards)
        {
            playerDeck.Add(card.cardName);
        }
    }
}
