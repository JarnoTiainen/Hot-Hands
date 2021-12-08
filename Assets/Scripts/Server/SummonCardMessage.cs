using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SummonCardMessage
{
    

    public enum CardSource
    {
        Hand,        //0
        Deck,        //1
        Void,        //2
        DiscardPile, //3
    }
    public int boardIndex;
    public int player;
    public bool auto;
    public bool free;
    public float attackCooldown;
    public string cardName;
    public int cardCost;
    public int cardValue;
    public int cardType;
    public List<Card.MonsterTag> mtag;
    public List<Card.SpellTag> stag;
    public int rp;
    public int lp;
    public List<Enchantment> enchantments;
    public string seed;
    public bool legendary;
    public CardSource cardSource;


    public SummonCardMessage(int boardIndex, int player, bool auto, bool free, float attackCooldown, CardData cardData)
    {
        this.boardIndex = boardIndex;
        this.player = player;
        this.auto = auto;
        this.free = free;
        this.attackCooldown = attackCooldown;
        this.cardName = cardData.cardName;
        this.cardCost = cardData.cost;
        this.cardValue = cardData.value;
        this.cardType = (int) cardData.cardType;
        this.mtag = cardData.monsterTags;
        this.stag = cardData.spellTags;
        this.rp = cardData.rp;
        this.lp = cardData.lp;
        this.enchantments = cardData.enchantments;
        this.seed = cardData.seed;
    }
}
