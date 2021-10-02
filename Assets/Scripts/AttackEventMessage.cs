using System;

[Serializable]
public class AttackEventMessage
{
    public int player;
    public string attacker;
    public string target;
    public CardPowersMessage attackerValues;
    public CardPowersMessage targetValues;
}
