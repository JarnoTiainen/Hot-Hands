using System;

[Serializable]
public class CardPowersMessage
{
    public string seed;
    public int rp;
    public int lp;

    public CardPowersMessage(string seed, int rp,int lp)
    {
        this.seed = seed;
        this.rp = rp;
        this.lp = lp;
    }
}
