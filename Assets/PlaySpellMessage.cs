using System.Collections.Generic;
using System;

[Serializable]
public class PlaySpellMessage
{
    public int player;
    public string seed;

    public string cardName;
    public int cardCost;
    public int cardValue;
    public int cardType;
    public List<Card.MonsterTag> mtag;
    public List<Card.SpellTag> stag;
    public List<Enchantment> enchantments;
}
