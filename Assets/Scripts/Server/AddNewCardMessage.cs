using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNewCardMessage
{
    public enum Destination
    {
        Default, Hand, Deck, Field, DiscardPile
    }

    public int player;
    public Destination destination;

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
}
