using System.Collections.Generic;
using System;

[Serializable]
public class GetDecksMessage
{
    /*
    public List<string> deck0;
    public List<string> deck1;
    public List<string> deck2;
    public List<string> deck3;
    public List<string> deck4;
    public string deck0Name;
    public string deck1Name;
    public string deck2Name;
    public string deck3Name;
    public string deck4Name;
    */

    public List<string> decks;
    public List<string> deckNames;
    public int activeDeckIndex;
    public List<Deck> convertedDecks;

    [Serializable]
    public class Deck
    {
        public List<string> deckContent;
    }
}
