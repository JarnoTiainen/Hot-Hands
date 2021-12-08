using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using SimpleJSON;
using System;



[Serializable]
public class Enchantment
{
    public string enchantmentName;
    public EnchantmentEffect enchantmentEffect;
    public Trigger trigger;
    public int weight;
    public bool targeting;
    public TargetType targetType;
    public CardDataMessage.BuffType buffType;
    public BuffVanishType buffVanishType;
    public bool hidden;

    public enum TargetType
    {
        Both, Ally, Enemy
    }

    public enum BuffVanishType
    {
        Permanent, All, Sacrifice, LastBreath
    }

    public enum EnchantmentEffect
    {
        Default,
        DrawCard, 
        GainBurn, 
        PlaySelf, 
        DiscardPileDuplication, 
        SummonToEnemy, 
        HealPlayer, 
        BuffBoardAttackDirectionPowers, 
        EssenceSiphon, 
        Scorch,
        Fireball,
        Deny,
        DestroyCombatant,
        BuffSelfPermanent,
        ReduceHighestCost,
        DamagePlayer,
        Vanish,
        AddSpellToHand,
        AddGolemToDiscardPile,
        BuffAllGolems,
        SpellDrawCard,
        StoneCarving,
        Purgatory,
        DivineDeluge,
        WinGame,
        StealTopCard,
        Sharphot,
        ArmorUp,
        Silence,
        Zap,
        BlessingOfTheDragon,
        BusinessDeal,
        SummonRat,
        GiantBribe,
        RatGrenade,
        GainRandomBurn,
        Pact
    }
    public enum Trigger
    {
        LastBreath,
        Opener,
        Battlecry,
        Drawtivation,
        Sacrifice,
        Retaliate,
        Brutality, 
        Spell
    }
    
}
