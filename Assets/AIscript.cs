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
                OpponentSummonCard();

                doOnce = false;
            }
        }
    }



    [Button] public void OpponentSummonCard()
    {
        StartCoroutine(WaitToSummon());
    }

    public void OpponentPlaySpell()
    {
        Debug.Log("opponent spell");
        StartCoroutine(OpponentSpell());
    }

    [Button] public void OpponentAttack()
    {

    }



    private IEnumerator WaitToSummon()
    {
        yield return new WaitForSeconds(delay);
        //there is something wrong with this, or not?
        CardData enemyCard = EnemyHand.Instance.GetComponentInChildren<InGameCard>().GetCardData();
        //CardData enemyCard = GameManager.Instance.GetCardFromInGameCards(TutorialManager.tutorialManagerInstance.enemyCardSeeds[TutorialManager.tutorialManagerInstance.enemyCardSeeds.Count - 1]).GetComponent<InGameCard>().GetCardData();
        Debug.Log("Enemy plays card " + enemyCard.cardName);


        bool free = enemyCard.cost == 0;
        SummonCardMessage summonCard = new SummonCardMessage(1, 1, false, free, TutorialManager.tutorialManagerInstance.attackCoolDown, enemyCard);
        GameManager.Instance.PlayerSummonCard(summonCard);
        if (TutorialManager.tutorialManagerInstance.GetState() == TutorialManager.TutorialState.CardPlay) {
            Debug.Log("nexstate from ai script");
            TutorialManager.tutorialManagerInstance.NextTutorialState();
        }
    }

    private IEnumerator OpponentSpell()
    {
        Debug.Log("opponent spell numerator");
        yield return new WaitUntil(() => TutorialManager.tutorialManagerInstance.firstSpell == true);
        //weird i know
        GameObject enemyCard = EnemyHand.Instance.transform.GetChild(0).gameObject;
        Debug.Log("opponent spell numerator2");
     
        //non targetting
        GameManager.Instance.enemyPlayerStats.playerBurnValue -= enemyCard.GetComponent<InGameCard>().GetData().cost;
        References.i.opponentBonfire.GetComponent<Bonfire>().burnValue.text = GameManager.Instance.enemyPlayerStats.playerBurnValue.ToString();
        Debug.Log("opponent spell numerator3");
        PlaySpellMessage playSpellMessage = new PlaySpellMessage(1, enemyCard.GetComponent<InGameCard>().GetCardData(), TutorialManager.tutorialManagerInstance.spellWindup);
        TutorialManager.tutorialManagerInstance.spellCardSeed.Add(enemyCard.GetComponent<InGameCard>().GetCardData().seed);
        playSpellMessage.slot = TutorialManager.tutorialManagerInstance.spellCardSeed.Count - 1;
        GameManager.Instance.PlaySpell(playSpellMessage);


        LimboCardHolder.Instance.StoreNewCard(enemyCard);


    }

    


}
