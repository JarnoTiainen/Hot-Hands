using System.Collections.Generic;
using System;

[Serializable]
public class CardDataMessage
{
    public enum BuffType
    {
        Default, StatBuff, StatDebuff, Damage
    }


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
    public string seed;
    public BuffType buffType;
}
