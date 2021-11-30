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

    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private GameObject damagePrefab;



    public void StartAttackEvent(bool wasYourAttack, CardPowersMessage attacker, CardPowersMessage target, float attackCD)
    {
        //trigger for the first attack in tutorial
        if(References.i.mouse.tutorialMode) {
            if(!TutorialManager.tutorialManagerInstance.firstAttack) {
                TutorialManager.tutorialManagerInstance.firstAttack = true;
                TutorialManager.tutorialManagerInstance.NextTutorialState();
            }
        }

        Debug.Log("starting attack event");

        //maybe put these ifs to start/awake?
        if (yourMonsterZone == null) yourMonsterZone = References.i.yourMonsterZone;
        if (opponentMonsterZone == null) opponentMonsterZone = References.i.opponentMonsterZone;


        if (wasYourAttack)
        {
            GameObject attackingCard = yourMonsterZone.GetCardWithSeed(attacker.seed);
            GameObject targetCard = opponentMonsterZone.GetCardWithSeed(target.seed);

            Debug.LogWarning("Attack: attacker: " + attackingCard.GetComponent<InGameCard>().GetData().cardName + " rp: " + attackingCard.GetComponent<InGameCard>().GetData().rp + " lp: " + attackingCard.GetComponent<InGameCard>().GetData().lp + " target: " + targetCard.GetComponent<InGameCard>().GetData().cardName + " rp: " + targetCard.GetComponent<InGameCard>().GetData().rp + " lp: " + targetCard.GetComponent<InGameCard>().GetData().lp);


            attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            GameManager.Instance.GetCardFromInGameCards(attacker.seed).GetComponent<InGameCard>().tempRp = attacker.rp;
            GameManager.Instance.GetCardFromInGameCards(attacker.seed).GetComponent<InGameCard>().tempLp = attacker.lp;
            GameManager.Instance.GetCardFromInGameCards(target.seed).GetComponent<InGameCard>().tempRp = target.lp;
            GameManager.Instance.GetCardFromInGameCards(target.seed).GetComponent<InGameCard>().tempLp = target.rp;
            attackingCard.GetComponent<InGameCard>().ToggleTrails(true);

            attackingCard.GetComponent<CardMovement>().OnCardAttack(targetCard, attackAnimationSpeed);
            
        }
        else
        {
            GameObject attackingCard = opponentMonsterZone.GetCardWithSeed(attacker.seed);
            GameObject targetCard = yourMonsterZone.GetCardWithSeed(target.seed);

            Debug.Log(" 2 Attack: attacker: " + attackingCard.GetComponent<InGameCard>().GetData().cardName + " rp: " + attackingCard.GetComponent<InGameCard>().GetData().rp + " lp: " + attackingCard.GetComponent<InGameCard>().GetData().lp + " target: " + targetCard.GetComponent<InGameCard>().GetData().cardName + " rp: " + targetCard.GetComponent<InGameCard>().GetData().rp + " lp: " + targetCard.GetComponent<InGameCard>().GetData().lp);

            attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            GameManager.Instance.GetCardFromInGameCards(attacker.seed).GetComponent<InGameCard>().tempRp = attacker.lp;
            GameManager.Instance.GetCardFromInGameCards(attacker.seed).GetComponent<InGameCard>().tempLp = attacker.rp;
            GameManager.Instance.GetCardFromInGameCards(target.seed).GetComponent<InGameCard>().tempRp = target.rp;
            GameManager.Instance.GetCardFromInGameCards(target.seed).GetComponent<InGameCard>().tempLp = target.lp;
            attackingCard.GetComponent<InGameCard>().ToggleTrails(true);

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
            attackingCard.GetComponent<InGameCard>().ToggleTrails(true);
            References.i.yourMonsterZone.GetCardWithSeed(attacker.seed).GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            GameManager.Instance.enemyPlayerStats.playerHealth -= playerTakenDamage;
        }
        else
        {
            GameObject attackingCard = References.i.opponentMonsterZone.GetCardWithSeed(attacker.seed);
            attackingCard.GetComponent<InGameCard>().ToggleAttackBurnEffect(false);
            attackingCard.GetComponent<CardMovement>().OnCardAttack(References.i.yourPlayerTarget, attackAnimationSpeed);
            attackingCard.GetComponent<InGameCard>().ToggleTrails(true);
            References.i.opponentMonsterZone.GetCardWithSeed(attacker.seed).GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            GameManager.Instance.playerStats.playerHealth -= playerTakenDamage;
        }
    }

    public void SpawnImpactEffect(Vector3 pos)
    {
        Debug.Log("Spawning effect");
        Instantiate(impactPrefab, pos, Quaternion.identity);
    }

    public bool StartDamageEvent(int player, GameObject attacker, GameObject target)
    {
        bool attackerDied = false;
        if (target == References.i.yourPlayerTarget || target == References.i.enemyPlayerTarget)
        {
            //Update player hp heal/trigger lose game event??
        }
        else
        {
            target.GetComponent<InGameCard>().SetTempValuesAsValues();
            attacker.GetComponent<InGameCard>().SetTempValuesAsValues();

            CardData targetCard = target.GetComponent<InGameCard>().GetData();
            CardData attackerCard = attacker.GetComponent<InGameCard>().GetData();
            attacker.GetComponent<InGameCard>().UpdateCardTexts();
            target.GetComponent<InGameCard>().UpdateCardTexts();
            bool wasYourAttack = GameManager.Instance.IsYou(player);

            if (wasYourAttack)
            {
                if (attackerCard.lp <= 0 || attackerCard.rp <= 0)
                {
                    GameManager.Instance.playerStats.playerFieldCards--;
                    //GameManager.Instance.RemoveCardFromInGameCards(attacker);
                    
                    References.i.yourMonsterZone.TryRemoveMonsterCard(attacker.GetComponent<InGameCard>().GetData().seed);
                    attackerDied = true;

                }
                if (targetCard.lp <= 0 || targetCard.rp <= 0)
                {
                    GameManager.Instance.enemyPlayerStats.playerFieldCards--;
                    //GameManager.Instance.RemoveCardFromInGameCards(target);
                    References.i.opponentMonsterZone.TryRemoveMonsterCard(target.GetComponent<InGameCard>().GetData().seed);
                }
            }
            else
            {
                if (attackerCard.lp <= 0 || attackerCard.rp <= 0)
                {
                    GameManager.Instance.enemyPlayerStats.playerFieldCards--;
                    //GameManager.Instance.RemoveCardFromInGameCards(attacker);
                    References.i.opponentMonsterZone.TryRemoveMonsterCard(attacker.GetComponent<InGameCard>().GetData().seed);
                    attackerDied = true;
                }
                if (targetCard.lp <= 0 || targetCard.rp <= 0)
                {
                    GameManager.Instance.playerStats.playerFieldCards--;
                    //GameManager.Instance.RemoveCardFromInGameCards(target);
                    References.i.yourMonsterZone.TryRemoveMonsterCard(target.GetComponent<InGameCard>().GetData().seed);
                }
            }
        }

        return attackerDied;
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
