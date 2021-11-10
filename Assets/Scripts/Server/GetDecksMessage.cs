using System.Collections.Generic;
using System;

[Serializable]
public class GetDecksMessage
{
    public List<Deck> decks;
    public int activeDeckIndex;

    [System.Serializable]
    public class Deck
    {
        public List<string> deck;
    }


}
