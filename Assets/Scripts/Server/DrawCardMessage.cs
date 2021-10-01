using System.Collections.Generic;
using System;
[Serializable]
public class DrawCardMessage
{
    public int player;
    public string cardName;
    public int cardCost;
    public int cardValue;
    public int cardType;
    public List<Card.MonsterTag> mtag;
    public List<Card.SpellTag> stag;
    public int rp;
    public int lp;


    public DrawCardMessage(int playerIn)
    {
        player = playerIn;
    }

    public DrawCardMessage(int player, string cardName, int cardCost, int cardValue, int cardType, List<Card.MonsterTag> mtag, List<Card.SpellTag> stag, int rp, int lp)
    {
        this.player = player;
        this.cardName = cardName;
        this.cardCost = cardCost;
        this.cardValue = cardValue;
        this.cardType = cardType;
        this.mtag = mtag;
        this.stag = stag;
        this.rp = rp;
        this.lp = lp;
    }
}
