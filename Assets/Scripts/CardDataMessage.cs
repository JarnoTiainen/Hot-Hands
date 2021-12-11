using System.Collections.Generic;
using System;

[Serializable]
public class CardDataMessage
{
    public enum BuffType
    {
        Default, StatBuff, StatDebuff, Damage
    }


    public string name;
    public int cost;
    public int value;
    public int cardtype;
    public int direction;
    public List<Card.MonsterTag> mtag;
    public List<Card.SpellTag> stag;
    public int rp;
    public int lp;
    public List<Enchantment> enchantments;
    public string seed;
    public BuffType buffType;
}
