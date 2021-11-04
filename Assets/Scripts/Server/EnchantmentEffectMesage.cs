using UnityEngine;
using System;


[Serializable]
public class EnchantmentEffectMesage
{
    public enum CardSource 
    {
        Default, Hand, Deck, Field, DiscardPile, Bonfire
    }

    public enum Trigger
    {
        LastBreath,
        Opener,
        Battlecry,
        Drawtivation,
        Sacrifice,
        Retaliate,
        Brutality
    }

    public string seed;
    public CardSource cardSource;
    public Trigger trigger;
    public DrawCardMessage cardInfo;
    public string card;
}
