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
    public CardDataMessage.BuffType buffType;
    public BuffVanishType buffVanishType;
    public bool hidden;

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
        BuffAllGolems
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
