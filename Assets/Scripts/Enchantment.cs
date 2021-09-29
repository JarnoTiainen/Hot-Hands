using System;
[Serializable]
public class Enchantment
{
    public string name;
    public EnchantmentEffect enchantmentEffect;
    public Trigger trigger;
    public int weight;



    public enum EnchantmentEffect
    {
        TestEffect
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
}
