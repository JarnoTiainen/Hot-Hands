using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    //This class is for moving data with cards from hand to field.


    public Sprite cardSprite;
    public string cardName;
    public int cost;
    public int value;
    public Card.CardType cardType;

    //info that only spells share
    public List<Card.SpellTag> spellTags;

    //info that only monsters share
    public List<Card.MonsterTag> monsterTags;
    public int rp;
    public int lp;

    [SerializeField] public List<Enchantment> enchantments = new List<Enchantment>();

    //add missing ones later
    public CardData(Sprite cardSprite, string cardName, int cost, int value, Card.CardType cardType, int rp, int lp) //List<Card.SpellTag> spellTags, List<Card.MonsterTag> monsterTags)
    {
        this.cardSprite = cardSprite;
        this.cardName = cardName;
        this.cost = cost;
        this.value = value;
        this.cardType = cardType;
        //this.spellTags = spellTags;
        //this.monsterTags = monsterTags;
        this.rp = rp;
        this.lp = lp;
    }
}
