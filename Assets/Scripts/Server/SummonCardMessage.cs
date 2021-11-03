using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SummonCardMessage
{
    public enum CardSource
    {
        Default,     //0
        Deck,        //1
        Hand,        //2
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
}
