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
    
    public DrawCardMessage(int player, string seed, float drawCooldown, Card card)
    {
        this.player = player;
        this.seed = seed;
        this.drawCooldown = drawCooldown;
        this.cardName = card.name;
        this.cardCost = card.cost;
        this.cardValue = card.value;
        this.cardType = (int) card.cardType;
        this.attackDirection = (int) card.attackDirection;
        this.mtag = card.monsterTags;
        this.stag = card.spellTags;
        this.rp = card.rp;
        this.lp = card.lp;
        this.enchantments = card.enchantments;
        
    }
}
