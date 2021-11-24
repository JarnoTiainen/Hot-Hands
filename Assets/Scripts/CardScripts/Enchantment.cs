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
        BuffSelfPermanent
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
