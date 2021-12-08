using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class CardData
{
    //This class is for moving data with cards from hand to field.


    public Sprite cardSprite;
    public string cardName;
    public int cost;
    public int value;
    public Card.CardType cardType;
    public Card.AttackDirection attackDirection;
    public bool legendary;
    //info that only spells share
    public List<Card.SpellTag> spellTags;

    //info that only monsters share
    public List<Card.MonsterTag> monsterTags;
    public int rp;
    public int lp;
    public string seed;
    public bool targetting;
    public Enchantment.TargetType targetType;
    [SerializeField] public List<Enchantment> enchantments = new List<Enchantment>();

    public string description;

    //add missing ones later
    public CardData(Sprite cardSprite, bool targetting, string cardName, int cost, int value, Card.CardType cardType, Card.AttackDirection attackDirection, int rp, int lp, List<Enchantment> enchantments, string seed, bool legendary, Enchantment.TargetType targetType) //List<Card.SpellTag> spellTags, List<Card.MonsterTag> monsterTags)
    {
        this.seed = seed;
        this.cardSprite = cardSprite;
        this.cardName = cardName;
        this.cost = cost;
        this.value = value;
        this.cardType = cardType;
        this.attackDirection = attackDirection;
        //this.spellTags = spellTags;
        //this.monsterTags = monsterTags;
        this.rp = rp;
        this.lp = lp;
        this.enchantments = enchantments;
        this.targetting = targetting;
        this.legendary = legendary;
        this.targetType = targetType;

    }

    public CardData(Sprite cardSprite, bool targetting, string cardName, int cost, int value, Card.CardType cardType, List<Enchantment> enchantments, string seed, bool legendary)
    {
        this.seed = seed;
        this.cardSprite = cardSprite;
        this.cardName = cardName;
        this.cost = cost;
        this.value = value;
        this.cardType = cardType;
        //this.spellTags = spellTags;
        //this.monsterTags = monsterTags;
        this.enchantments = enchantments;
        this.targetting = targetting;
        this.description = description;
        this.legendary = legendary;
    }
}
