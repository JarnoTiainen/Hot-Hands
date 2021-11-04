using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class AttackEventHandler : MonoBehaviour
{
    MonsterZone yourMonsterZone;
    MonsterZone opponentMonsterZone;
    public float attackAnimationSpeed = 0.4f;
    public float shakeMagnitude = 1f;
    public float shakeRoughness = 4f;
    public float fadeInTime = .1f;
    public float fadeOutTime = 1f;
    public AnimationCurve shakeCurve;

    public void StartAttackEvent(bool wasYourAttack, CardPowersMessage attacker, CardPowersMessage target, float attackCD)
    {
        Debug.Log("starting attack event");

        //maybe put these ifs to start/awake?
        if (yourMonsterZone == null) yourMonsterZone = References.i.yourMonsterZone;
        if (opponentMonsterZone == null) opponentMonsterZone = References.i.opponentMonsterZone;


        if (wasYourAttack)
        {
            GameObject attackingCard = yourMonsterZone.GetCardWithSeed(attacker.seed);
            GameObject targetCard = opponentMonsterZone.GetCardWithSeed(target.seed);

            Debug.LogWarning("Attack: attacker: " + attackingCard.GetComponent<InGameCard>().cardData.cardName + " rp: " + attackingCard.GetComponent<InGameCard>().cardData.rp + " lp: " + attackingCard.GetComponent<InGameCard>().cardData.lp + " target: " + targetCard.GetComponent<InGameCard>().cardData.cardName + " rp: " + targetCard.GetComponent<InGameCard>().cardData.rp + " lp: " + targetCard.GetComponent<InGameCard>().cardData.lp);


            attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            References.i.yourMonsterZone.UpdateCardData(wasYourAttack, attacker);
            References.i.opponentMonsterZone.UpdateCardData(!wasYourAttack, target);

            attackingCard.GetComponent<CardMovement>().OnCardAttack(targetCard, attackAnimationSpeed);
            
        }
        else
        {
            GameObject attackingCard = opponentMonsterZone.GetCardWithSeed(attacker.seed);
            GameObject targetCard = yourMonsterZone.GetCardWithSeed(target.seed);

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
            GameObject attackingCard = References.i.yourMonsterZone.GetCardWithSeed(attacker.seed);
            attackingCard.GetComponent<CardMovement>().OnCardAttack(References.i.enemyPlayerTarget, attackAnimationSpeed);
            References.i.yourMonsterZone.GetCardWithSeed(attacker.seed).GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            GameManager.Instance.enemyPlayerStats.playerHealth -= playerTakenDamage;
        }
        else
        {
            GameObject attackingCard = References.i.opponentMonsterZone.GetCardWithSeed(attacker.seed);
            attackingCard.GetComponent<InGameCard>().ToggleAttackBurnEffect(false);
            attackingCard.GetComponent<CardMovement>().OnCardAttack(References.i.yourPlayerTarget, attackAnimationSpeed);
            References.i.opponentMonsterZone.GetCardWithSeed(attacker.seed).GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            GameManager.Instance.playerStats.playerHealth -= playerTakenDamage;
        }
    }

    public void StartDamageEvent(int player, GameObject attacker, GameObject target)
    {

        if (target == References.i.yourPlayerTarget || target == References.i.enemyPlayerTarget)
        {
            //Update player hp heal/trigger lose game event??
        }
        else
        {
            CardData targetCard = target.GetComponent<InGameCard>().cardData;
            CardData attackerCard = attacker.GetComponent<InGameCard>().cardData;
            attacker.GetComponent<InGameCard>().UpdateCardTexts();
            target.GetComponent<InGameCard>().UpdateCardTexts();
            bool wasYourAttack = GameManager.Instance.IsYou(player);

            if (wasYourAttack)
            {
                if (attackerCard.lp <= 0 || attackerCard.rp <= 0)
                {
                    GameManager.Instance.playerStats.playerFieldCards--;
                    References.i.yourMonsterZone.limboCards.Remove(attacker);
                    GameManager.Instance.RemoveCardFromInGameCards(attacker);
                    Destroy(attacker);
                }
                if (targetCard.lp <= 0 || targetCard.rp <= 0)
                {
                    GameManager.Instance.enemyPlayerStats.playerFieldCards--;
                    References.i.yourMonsterZone.limboCards.Remove(target);
                    GameManager.Instance.RemoveCardFromInGameCards(target);
                    Destroy(target);
                }
            }
            else
            {
                if (attackerCard.lp <= 0 || attackerCard.rp <= 0)
                {
                    GameManager.Instance.enemyPlayerStats.playerFieldCards--;
                    References.i.yourMonsterZone.limboCards.Remove(attacker);
                    GameManager.Instance.RemoveCardFromInGameCards(attacker);
                    Destroy(attacker);
                }
                if (targetCard.lp <= 0 || targetCard.rp <= 0)
                {
                    GameManager.Instance.playerStats.playerFieldCards--;
                    References.i.yourMonsterZone.limboCards.Remove(target);
                    GameManager.Instance.RemoveCardFromInGameCards(target);
                    Destroy(target);
                }
            }
        }
    }

    public void CameraShake(CardData attackerCard)
    {
        if (attackerCard.attackDirection == Card.AttackDirection.Right) {
            CameraShaker.Instance.ShakeOnce(shakeCurve.Evaluate(attackerCard.rp * shakeMagnitude), shakeRoughness, fadeInTime, fadeOutTime);
        } else {
            CameraShaker.Instance.ShakeOnce(shakeCurve.Evaluate(attackerCard.lp * shakeMagnitude), shakeRoughness, fadeInTime, fadeOutTime);
        }
    }

}
