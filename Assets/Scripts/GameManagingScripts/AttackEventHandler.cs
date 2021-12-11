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
        

        Debug.Log("starting attack event");

        //maybe put these ifs to start/awake?
        if (yourMonsterZone == null) yourMonsterZone = References.i.yourMonsterZone;
        if (opponentMonsterZone == null) opponentMonsterZone = References.i.opponentMonsterZone;


        if (wasYourAttack)
        {
            GameObject attackingCard = yourMonsterZone.GetCardWithSeed(attacker.seed);
            GameObject targetCard = opponentMonsterZone.GetCardWithSeed(target.seed);

            if(attackingCard != null)
            {
                Debug.LogWarning("Attack: attacker: " + attackingCard.GetComponent<InGameCard>().GetData().cardName + " rp: " + attackingCard.GetComponent<InGameCard>().GetData().rp + " lp: " + attackingCard.GetComponent<InGameCard>().GetData().lp);
                attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
                GameManager.Instance.GetCardFromInGameCards(attacker.seed).GetComponent<InGameCard>().tempRp = attacker.rp;
                GameManager.Instance.GetCardFromInGameCards(attacker.seed).GetComponent<InGameCard>().tempLp = attacker.lp;

                attackingCard.GetComponent<InGameCard>().ToggleTrails(true);
                attackingCard.GetComponent<CardMovement>().OnCardAttack(targetCard, attackAnimationSpeed);
            }
            if(targetCard != null)
            {
                Debug.LogWarning("Attack target: " + targetCard.GetComponent<InGameCard>().GetData().cardName + " rp: " + targetCard.GetComponent<InGameCard>().GetData().rp + " lp: " + targetCard.GetComponent<InGameCard>().GetData().lp);
                GameManager.Instance.GetCardFromInGameCards(target.seed).GetComponent<InGameCard>().tempRp = target.lp;
                GameManager.Instance.GetCardFromInGameCards(target.seed).GetComponent<InGameCard>().tempLp = target.rp;
            }
            
        }
        else
        {
            GameObject attackingCard = opponentMonsterZone.GetCardWithSeed(attacker.seed);
            GameObject targetCard = yourMonsterZone.GetCardWithSeed(target.seed);

            if(attackingCard != null)
            {
                Debug.LogWarning("Attack: attacker: " + attackingCard.GetComponent<InGameCard>().GetData().cardName + " rp: " + attackingCard.GetComponent<InGameCard>().GetData().rp + " lp: " + attackingCard.GetComponent<InGameCard>().GetData().lp);
                GameManager.Instance.GetCardFromInGameCards(attacker.seed).GetComponent<InGameCard>().tempRp = attacker.lp;
                GameManager.Instance.GetCardFromInGameCards(attacker.seed).GetComponent<InGameCard>().tempLp = attacker.rp;
                attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
                attackingCard.GetComponent<InGameCard>().ToggleTrails(true);
                attackingCard.GetComponent<CardMovement>().OnCardAttack(targetCard, attackAnimationSpeed);
                attackingCard.GetComponent<InGameCard>().ToggleAttackBurnEffect(false);
            }
            if(targetCard != null)
            {
                Debug.LogWarning("Attack target: " + targetCard.GetComponent<InGameCard>().GetData().cardName + " rp: " + targetCard.GetComponent<InGameCard>().GetData().rp + " lp: " + targetCard.GetComponent<InGameCard>().GetData().lp);
                GameManager.Instance.GetCardFromInGameCards(target.seed).GetComponent<InGameCard>().tempRp = target.rp;
                GameManager.Instance.GetCardFromInGameCards(target.seed).GetComponent<InGameCard>().tempLp = target.lp;
            }
        }

        //if (References.i.mouse.tutorialMode) {
        //    if (!TutorialManager.tutorialManagerInstance.firstAttack) {
        //        Debug.Log("First attack!");
        //        TutorialManager.tutorialManagerInstance.firstAttack = true;
        //        TutorialManager.tutorialManagerInstance.NextTutorialState();
        //    }             
        //}
      
    }

    public void StartAttackEvent(bool wasYourAttack, CardPowersMessage attacker, int playerTakenDamage, float attackCD)
    {
        if (wasYourAttack)
        {
            GameObject attackingCard = References.i.yourMonsterZone.GetCardWithSeed(attacker.seed);
            if(attackingCard != null)
            {
                attackingCard.GetComponent<CardMovement>().OnCardAttack(References.i.enemyPlayerTarget, attackAnimationSpeed);
                attackingCard.GetComponent<InGameCard>().ToggleTrails(true);
                attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            }
        }
        else
        {
            GameObject attackingCard = References.i.opponentMonsterZone.GetCardWithSeed(attacker.seed);
            if(attackingCard != null)
            {
                attackingCard.GetComponent<InGameCard>().ToggleAttackBurnEffect(false);
                attackingCard.GetComponent<CardMovement>().OnCardAttack(References.i.yourPlayerTarget, attackAnimationSpeed);
                attackingCard.GetComponent<InGameCard>().ToggleTrails(true);
                attackingCard.GetComponent<InGameCard>().StartAttackCooldown(attackCD);
            }
        }

        if (References.i.mouse.tutorialMode) {

            if (!TutorialManager.tutorialManagerInstance.firstDirectAttack) {
                if (playerTakenDamage != 0) {
                    Debug.Log("First direct attack!");
                    TutorialManager.tutorialManagerInstance.firstDirectAttack = true;
                    TutorialManager.tutorialManagerInstance.NextTutorialState();
                }
            } 
            
        }


    }

    public void SpawnImpactEffect(Vector3 pos)
    {
        if(!References.i.mouse.tutorialMode) {
            Debug.Log("Spawning effect");
            Instantiate(impactPrefab, pos, Quaternion.identity);
        } else {
            //TODO: change this
            if(TutorialManager.tutorialManagerInstance.firstAttack) {
                Debug.Log("Spawning effect");
                Instantiate(impactPrefab, pos, Quaternion.identity);
            
            }
        }
        
    }

    public bool StartDamageEvent(int player, GameObject attacker, GameObject target)
    {
        //trigger for the first attack in tutorial
        if(References.i.mouse.tutorialMode) {
            if (!TutorialManager.tutorialManagerInstance.firstAttack) {
                Debug.Log("scaling timwe");
                Time.timeScale = 0f;
                TutorialManager.tutorialManagerInstance.NextTutorialState();
                StartCoroutine(StartDamageEventTutorial(player, attacker, target));
                return false;
            }
            
        }
 

        bool attackerDied = false;
        if (target == References.i.yourPlayerTarget || target == References.i.enemyPlayerTarget)
        {
            //Update player hp heal/trigger lose game event for tutorial
            if (References.i.mouse.tutorialMode) {
                if (target == References.i.yourPlayerTarget) 
                {
                    if(attacker != null)
                    {
                        if (attacker.GetComponent<InGameCard>().GetData().attackDirection == Card.AttackDirection.Left)
                        {
                            GameManager.Instance.playerStats.playerHealth -= attacker.GetComponent<InGameCard>().GetData().lp;
                        }
                        else if (attacker.GetComponent<InGameCard>().GetData().attackDirection == Card.AttackDirection.Right)
                        {
                            GameManager.Instance.playerStats.playerHealth -= attacker.GetComponent<InGameCard>().GetData().rp;
                        }
                    }
                    
                    
                } 
                else if (target == References.i.enemyPlayerTarget) 
                {
                    if(attacker != null)
                    {
                        if (attacker.GetComponent<InGameCard>().GetData().attackDirection == Card.AttackDirection.Left)
                        {
                            GameManager.Instance.enemyPlayerStats.playerHealth -= attacker.GetComponent<InGameCard>().GetData().lp;
                        }
                        else if (attacker.GetComponent<InGameCard>().GetData().attackDirection == Card.AttackDirection.Right)
                        {
                            GameManager.Instance.enemyPlayerStats.playerHealth -= attacker.GetComponent<InGameCard>().GetData().rp;
                        }
                    }
                }
            }

        }
        else
        {
            bool wasYourAttack = GameManager.Instance.IsYou(player);
            CardData attackerCard= null;
            CardData targetCard = null;
            if (target != null)
            {
                target.GetComponent<InGameCard>().SetTempValuesAsValues();
                targetCard = target.GetComponent<InGameCard>().GetData();
                target.GetComponent<InGameCard>().UpdateCardTexts();
                
            }
            if(attacker != null)
            {
                attacker.GetComponent<InGameCard>().SetTempValuesAsValues();
                attackerCard = attacker.GetComponent<InGameCard>().GetData();
                attacker.GetComponent<InGameCard>().UpdateCardTexts();
            }
            

            if (wasYourAttack)
            {
                if(attacker != null)
                {
                    if (attackerCard.lp <= 0 || attackerCard.rp <= 0)
                    {
                        GameManager.Instance.playerStats.playerFieldCards--;
                        //GameManager.Instance.RemoveCardFromInGameCards(attacker);

                        References.i.yourMonsterZone.TryRemoveMonsterCard(attacker.GetComponent<InGameCard>().GetData().seed);
                        attackerDied = true;

                    }
                }
                if(target != null)
                {
                    if (targetCard.lp <= 0 || targetCard.rp <= 0)
                    {
                        GameManager.Instance.enemyPlayerStats.playerFieldCards--;
                        //GameManager.Instance.RemoveCardFromInGameCards(target);
                        References.i.opponentMonsterZone.TryRemoveMonsterCard(target.GetComponent<InGameCard>().GetData().seed);
                    }
                }
            }
            else
            {
                if(attacker != null)
                {
                    if (attackerCard.lp <= 0 || attackerCard.rp <= 0)
                    {
                        GameManager.Instance.enemyPlayerStats.playerFieldCards--;
                        //GameManager.Instance.RemoveCardFromInGameCards(attacker);
                        References.i.opponentMonsterZone.TryRemoveMonsterCard(attacker.GetComponent<InGameCard>().GetData().seed);
                        attackerDied = true;
                    }
                }
                if(target != null)
                {
                    if (targetCard.lp <= 0 || targetCard.rp <= 0)
                    {
                        GameManager.Instance.playerStats.playerFieldCards--;
                        //GameManager.Instance.RemoveCardFromInGameCards(target);
                        References.i.yourMonsterZone.TryRemoveMonsterCard(target.GetComponent<InGameCard>().GetData().seed);
                    }
                }
            }
        }

        return attackerDied;
    }

    public IEnumerator StartDamageEventTutorial(int player, GameObject attacker, GameObject target)
    {
        yield return new WaitUntil(() => TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.DirectAttack);
        Time.timeScale = 1;

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
                }
                if (targetCard.lp <= 0 || targetCard.rp <= 0)
                {
                    GameManager.Instance.playerStats.playerFieldCards--;
                    //GameManager.Instance.RemoveCardFromInGameCards(target);
                    References.i.yourMonsterZone.TryRemoveMonsterCard(target.GetComponent<InGameCard>().GetData().seed);
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
