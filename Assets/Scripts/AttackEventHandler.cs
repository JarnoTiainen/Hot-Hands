using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEventHandler : MonoBehaviour
{
    MonsterZone yourMonsterZone;
    MonsterZone opponentMonsterZone;
    public float attackAnimationSpeed = 0.4f;

    public void StartAttackEvent(bool wasYourAttack, CardPowersMessage attacker, CardPowersMessage target, float attackCD)
    {
        if (yourMonsterZone == null) yourMonsterZone = References.i.yourMonsterZone;
        if (opponentMonsterZone == null) opponentMonsterZone = References.i.opponentMonsterZone;


        if (wasYourAttack)
        {
            GameObject attackingCard = yourMonsterZone.GetCardWithServerIndex(attacker.index);
            GameObject targetCard = opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(target.index));


            attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            References.i.yourMonsterZone.UpdateCardData(wasYourAttack, attacker);
            References.i.opponentMonsterZone.UpdateCardData(!wasYourAttack, target);

            attackingCard.GetComponent<CardMovement>().OnCardAttack(targetCard, attackAnimationSpeed);
        }
        else
        {
            GameObject attackingCard = opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(attacker.index));
            GameObject targetCard = yourMonsterZone.GetCardWithServerIndex(target.index);


            attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            References.i.yourMonsterZone.UpdateCardData(!wasYourAttack, target);
            References.i.opponentMonsterZone.UpdateCardData(wasYourAttack, attacker);

            attackingCard.GetComponent<CardMovement>().OnCardAttack(targetCard, attackAnimationSpeed);

        }
    }
    public void StartDamageEvent(int player, GameObject attacker, GameObject target)
    {
        Debug.Log(player + " " + attacker.GetComponent<InGameCard>().cardData.cardName + " " + target.GetComponent<InGameCard>().cardData.cardName);


        CardData attackerCard = attacker.GetComponent<InGameCard>().cardData;
        CardData targetCard = target.GetComponent<InGameCard>().cardData;

        attacker.GetComponent<InGameCard>().UpdateCardTexts();
        target.GetComponent<InGameCard>().UpdateCardTexts();

        bool wasYourAttack = GameManager.Instance.IsYou(player);


        Debug.Log("attacker: " + attackerCard.rp + " " + attackerCard.lp + " target: " + targetCard.rp + " " + targetCard.lp);
        if (wasYourAttack)
        {
            if (attackerCard.lp <= 0 || attackerCard.rp <= 0)
            {
                GameManager.Instance.playerStats.playerFieldCards--;
                References.i.yourMonsterZone.RemoveMonsterCard(attacker.GetComponent<InGameCard>().serverConfirmedIndex);
            }
            if (targetCard.lp <= 0 || targetCard.rp <= 0)
            {
                GameManager.Instance.enemyPlayerStats.playerFieldCards--;
                References.i.opponentMonsterZone.RemoveMonsterCard(target.GetComponent<InGameCard>().serverConfirmedIndex);
            }
        }
        else
        {
            if (attackerCard.lp <= 0 || attackerCard.rp <= 0)
            {
                GameManager.Instance.enemyPlayerStats.playerFieldCards--;
                References.i.opponentMonsterZone.RemoveMonsterCard(attacker.GetComponent<InGameCard>().serverConfirmedIndex);
            }
            if (targetCard.lp <= 0 || targetCard.rp <= 0)
            {
                GameManager.Instance.playerStats.playerFieldCards--;
                References.i.yourMonsterZone.RemoveMonsterCard(target.GetComponent<InGameCard>().serverConfirmedIndex);
            }
        }
    }
}
