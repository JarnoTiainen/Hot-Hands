using System.Collections.Generic;
using UnityEngine;
public class CardJSON
{
    public string name;
    public int cost;
    public int value;
    public Card.CardType cardType;
    public List<Card.SpellTag> spellTags;
    public List<Card.MonsterTag> monsterTags;
    public int rp;
    public int lp;
    public Card.AttackDirection attackDirection;
    public List<Enchantment> enchantments;

    public CardJSON(string name, int cost, int value, Card.CardType cardType, List<Card.SpellTag> spellTags, List<Card.MonsterTag> monsterTags, int rp, int lp, Card.AttackDirection attackDirection, List<Enchantment> enchantments)
    {
        this.name = name;
        this.cost = cost;
        this.value = value;
        this.cardType = cardType;
        this.spellTags = spellTags;
        this.monsterTags = monsterTags;
        this.rp = rp;
        this.lp = lp;
        this.attackDirection = attackDirection;
        this.enchantments = enchantments;
    }

}
