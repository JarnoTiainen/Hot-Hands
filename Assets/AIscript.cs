using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIscript : MonoBehaviour
{
    public bool doOnce = true;
    int drawnCards = 0;
    public float delay = 1.5f;

    private void Update()
    {
        if (TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.CardDraw && drawnCards == 0) {
            Debug.Log("Enemy draws card");
            References.i.opponentDeck.GetComponent<TutorialDeck>().OpponentDraw();
            References.i.opponentDeck.GetComponent<TutorialDeck>().OpponentDraw();
            drawnCards++;
            
        }
        
        if (TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.CardPlay && doOnce) {
            if (GameManager.Instance.playerStats.playerFieldCards == 1) {
                StartCoroutine(WaitToSummon());   
                doOnce = false;
            }
            
        }

    }

    [Button] public void OpponentSummonCard()
    {
        //there is something wrong with this
         CardData enemyCard = EnemyHand.Instance.GetComponentInChildren<InGameCard>().GetCardData();
        //CardData enemyCard = GameManager.Instance.GetCardFromInGameCards(TutorialManager.tutorialManagerInstance.enemyCardSeeds[TutorialManager.tutorialManagerInstance.enemyCardSeeds.Count - 1]).GetComponent<InGameCard>().GetCardData();
        Debug.Log("Enemy plays card " + enemyCard.cardName);
            
                    
        bool free = enemyCard.cost == 0;
        SummonCardMessage summonCard = new SummonCardMessage(1, 1, false, free, TutorialManager.tutorialManagerInstance.attackCoolDown, enemyCard);
        GameManager.Instance.PlayerSummonCard(summonCard);
    }

    [Button] public void OpponentAttack()
    {

    }

    private IEnumerator WaitToSummon()
    {
        yield return new WaitForSeconds(delay);
        OpponentSummonCard();
        if(TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.CardPlay) {
            TutorialManager.tutorialManagerInstance.NextTutorialState();
        }
    }
}
