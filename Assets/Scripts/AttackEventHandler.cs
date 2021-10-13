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
        Debug.Log("starting attack event");

        //maybe put these ifs to start/awake?
        if (yourMonsterZone == null) yourMonsterZone = References.i.yourMonsterZone;
        if (opponentMonsterZone == null) opponentMonsterZone = References.i.opponentMonsterZone;


        if (wasYourAttack)
        {
            GameObject attackingCard = yourMonsterZone.GetCardWithServerIndex(attacker.index);
            GameObject targetCard = opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(target.index));

            Debug.LogWarning("Attack: attacker: " + attackingCard.GetComponent<InGameCard>().cardData.cardName + " rp: " + attackingCard.GetComponent<InGameCard>().cardData.rp + " lp: " + attackingCard.GetComponent<InGameCard>().cardData.lp + " target: " + targetCard.GetComponent<InGameCard>().cardData.cardName + " rp: " + targetCard.GetComponent<InGameCard>().cardData.rp + " lp: " + targetCard.GetComponent<InGameCard>().cardData.lp);


            attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            References.i.yourMonsterZone.UpdateCardData(wasYourAttack, attacker);
            References.i.opponentMonsterZone.UpdateCardData(!wasYourAttack, target);

            attackingCard.GetComponent<CardMovement>().OnCardAttack(targetCard, attackAnimationSpeed);
        }
        else
        {
            GameObject attackingCard = opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(attacker.index));
            GameObject targetCard = yourMonsterZone.GetCardWithServerIndex(target.index);

            Debug.Log(" 2 Attack: attacker: " + attackingCard.GetComponent<InGameCard>().cardData.cardName + " rp: " + attackingCard.GetComponent<InGameCard>().cardData.rp + " lp: " + attackingCard.GetComponent<InGameCard>().cardData.lp + " target: " + targetCard.GetComponent<InGameCard>().cardData.cardName + " rp: " + targetCard.GetComponent<InGameCard>().cardData.rp + " lp: " + targetCard.GetComponent<InGameCard>().cardData.lp);

            attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            References.i.yourMonsterZone.UpdateCardData(!wasYourAttack, target);
            References.i.opponentMonsterZone.UpdateCardData(wasYourAttack, attacker);

            attackingCard.GetComponent<CardMovement>().OnCardAttack(targetCard, attackAnimationSpeed);
            attackingCard.GetComponent<InGameCard>().ToggleAttackBurnEffect(false);
        }
    }

    public void StartAttackEvent(bool wasYourAttack, CardPowersMessage attacker, int playerTakenDamage, float attackCD)
    {
        if (wasYourAttack)
        {



            GameObject attackingCard = References.i.yourMonsterZone.GetCardWithServerIndex(attacker.index);
            attackingCard.GetComponent<CardMovement>().OnCardAttack(References.i.enemyPlayerTarget, attackAnimationSpeed);
            References.i.yourMonsterZone.GetCardWithServerIndex(attacker.index).GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            GameManager.Instance.enemyPlayerStats.playerHealth -= playerTakenDamage;


        }
        else
        {
            GameObject attackingCard = References.i.opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(attacker.index));
            attackingCard.GetComponent<InGameCard>().ToggleAttackBurnEffect(false);
            attackingCard.GetComponent<CardMovement>().OnCardAttack(References.i.yourPlayerTarget, attackAnimationSpeed);
            References.i.opponentMonsterZone.GetCardWithServerIndex(References.i.opponentMonsterZone.RevertIndex(attacker.index)).GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            GameManager.Instance.playerStats.playerHealth -= playerTakenDamage;
        }
    }

    public void StartDamageEvent(int player, GameObject attacker, GameObject target)
    {
        if(target == References.i.yourPlayerTarget || target == References.i.enemyPlayerTarget)
        {
            //Update player hp heal/trigger lose game event
        }
        else
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
}
