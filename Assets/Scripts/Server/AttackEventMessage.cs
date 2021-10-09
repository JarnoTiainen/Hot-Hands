using System;

[Serializable]
public class AttackEventMessage
{
    public bool directHit;
    public int playerTakenDamage;
    public int player;
    public string attacker;
    public string target;
    public CardPowersMessage attackerValues;
    public CardPowersMessage targetValues;
}
