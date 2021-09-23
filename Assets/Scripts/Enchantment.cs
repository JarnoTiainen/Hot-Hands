using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment
{
    public string name;
    public string description;
    public Trigger trigger;
    public IEnchantmentEffect enchantmentEffect;

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
}
