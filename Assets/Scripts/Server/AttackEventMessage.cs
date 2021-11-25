using System;

[Serializable]
public class AttackEventMessage
{
    public bool denied;
    public float attackCooldown;
    public bool directHit;
    public int playerTakenDamage;
    public int player;
    public string attacker;
    public string target;
    public CardPowersMessage attackerValues;
    public CardPowersMessage targetValues;

    public AttackEventMessage(bool denied, float attackCooldown, bool directHit, int playerTakenDamage, int player, CardPowersMessage attackerValues, CardPowersMessage targetValues)
    {
        this.denied = denied;
        this.attackCooldown = attackCooldown;
        this.directHit = directHit;
        this.playerTakenDamage = playerTakenDamage;
        this.player = player;
        this.attackerValues = attackerValues;
        this.targetValues = targetValues;
    }
}
