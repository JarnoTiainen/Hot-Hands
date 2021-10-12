using System;

[Serializable]
public class CardPowersMessage
{
    public int index;
    public int rp;
    public int lp;

    public CardPowersMessage(int index, int rp,int lp)
    {
        this.index = index;
        this.rp = rp;
        this.lp = lp;
    }
}
