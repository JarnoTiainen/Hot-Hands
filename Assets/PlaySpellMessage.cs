using System.Collections.Generic;
using System;

[Serializable]
public class PlaySpellMessage
{
    public int player;
    public string seed;

    public string cardName;
    public int cardCost;
    public int cardValue;
    public int cardType;
    public List<Card.MonsterTag> mtag;
    public List<Card.SpellTag> stag;
    public List<Enchantment> enchantments;
    public float windup;
    public int slot;
    public bool legendary;

    public List<string> targets;

    public PlaySpellMessage(int player, CardData card, float windup, string target = "")
    {
        
        this.player = player;
        this.seed = card.seed;
        this.cardName = card.cardName;
        this.cardCost = card.cost;
        this.cardValue = card.value;
        this.cardType = (int) card.cardType;
        this.mtag = card.monsterTags;
        this.stag = card.spellTags;
        this.enchantments = card.enchantments;
        this.windup = windup;
        this.legendary = card.legendary;
        targets = new List<string>();
        if (target != "") {
            this.targets.Add(target);
        } 
        
    }
    
}
