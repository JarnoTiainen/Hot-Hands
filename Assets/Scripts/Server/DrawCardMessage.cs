using System.Collections.Generic;
using System;
[Serializable]
public class DrawCardMessage
{
    public int player;
    public float drawCooldown;
    public string cardName;
    public int cardCost;
    public int cardValue;
    public int cardType;
    public int attackDirection;
    public List<Card.MonsterTag> mtag;
    public List<Card.SpellTag> stag;
    public int rp;
    public int lp;
    public List<Enchantment> enchantments;
    public string seed;


    public DrawCardMessage(int playerIn)
    {
        player = playerIn;
    }

    public DrawCardMessage(int player, string cardName, int cardCost, int cardValue, int cardType, int attackDirection, List<Card.MonsterTag> mtag, List<Card.SpellTag> stag, int rp, int lp, List<Enchantment> enchantments, string seed, float drawCooldown)
    {
        this.player = player;
        this.cardName = cardName;
        this.cardCost = cardCost;
        this.cardValue = cardValue;
        this.cardType = cardType;
        this.attackDirection = attackDirection;
        this.mtag = mtag;
        this.stag = stag;
        this.rp = rp;
        this.lp = lp;
        this.enchantments = enchantments;
        this.seed = seed;
        this.drawCooldown = drawCooldown;
    }
}
