using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MonsterZone : MonoBehaviour
{
    public List<GameObject> serverConfirmedCards = new List<GameObject>();
    public List<GameObject> unhandledCards = new List<GameObject>();
    public List<GameObject> monsterCards = new List<GameObject>();
    Dictionary<GameObject, float> cardXposDictionary = new Dictionary<GameObject, float>();
    [SerializeField] private float gapBetweenCards;
    [SerializeField] private bool debugModeOn = false;
    public bool isYourMonsterZone;
    [SerializeField] [ShowIf("debugModeOn", true)]public GameObject ghostCard;
    public List<GameObject> limboCards = new List<GameObject>();


    private void Update()
    {
        if (Mouse.Instance.heldCard != null && isYourMonsterZone && GameManager.Instance.playerStats.playerFieldCards < GameManager.Instance.maxFieldCardCount)
        {
            MakeRoom();
        }
    }

    [Button]public void AddNewMonsterCard(bool isYourCard, int boardIndex, CardData data)
    {
        if(isYourCard)
        {
            ghostCard.GetComponent<InGameCard>().ToggleGhostCard();
            ghostCard.GetComponent<InGameCard>().SetNewCardData(isYourCard, data);
            unhandledCards.Add(ghostCard);
            if(debugModeOn) Debug.Log("Added unhandled");
            ghostCard = null;
            //ReCalculateCardIndexes();
        } else {
            
            //enemycard
            int index;
            if (monsterCards.Count == 0) {
                index = 0;
            } else {
                index = monsterCards.Count - boardIndex;
            }

            GameObject newMonster = Instantiate(References.i.fieldCard);
            serverConfirmedCards.Add(newMonster);
            newMonster.transform.SetParent(transform);
            if (debugModeOn) Debug.Log("index: " + index);
            monsterCards.Insert(index, newMonster);
            newMonster.GetComponent<InGameCard>().SetNewCardData(isYourCard, data);
            ReCalculateServerCardIndexes();
            RepositionMonsterCards();
        }
    }

    public void RemoveMonsterCard(int index)
    {
        if (debugModeOn) Debug.Log("Removing monster " + index);
        GameObject deadMonster = GetCardWithServerIndex(index);
        monsterCards.Remove(deadMonster);
        serverConfirmedCards.Remove(deadMonster);
        GameObject.Destroy(deadMonster);
        ReCalculateServerCardIndexes();
        RepositionMonsterCards();

    }

    public void RemoveMonsterCardNoDestroy(int index)
    {

        if (debugModeOn) Debug.Log("Removing monster " + index);
        GameObject deadMonster = GetCardWithServerIndex(index);
        limboCards.Add(deadMonster);
        monsterCards.Remove(deadMonster);
        serverConfirmedCards.Remove(deadMonster);
        ReCalculateServerCardIndexes();
        RepositionMonsterCards();

    }
    public GameObject FindLimboCardWithIndex(bool isYourCard, int index)
    {
        foreach(GameObject card in limboCards)
        {
            if(GameManager.Instance.IsYou(card.GetComponent<InGameCard>().owner) && isYourCard && card.GetComponent<InGameCard>().serverConfirmedIndex == index)
            {
                return card;
            }
            else if(!GameManager.Instance.IsYou(card.GetComponent<InGameCard>().owner) && isYourCard && card.GetComponent<InGameCard>().serverConfirmedIndex == index)
            {
                return card;
            }
        }

        Debug.LogError("LIMBO CARD WAS NOT FOUND WITH INDEX: " + index);
        return null;
    }


    //public void ReCalculateCardIndexes()
    //{
    //    Debug.Log("ReCalculateCardIndexes");
    //    int currentIndex = 0;
    //    foreach(GameObject card in monsterCards)
    //    {
    //        if(ghostCard)
    //        {
    //            if(card != ghostCard)
    //            {
    //                card.GetComponent<InGameCard>().indexOnField = currentIndex;
    //                currentIndex++;
    //            }
    //        }
    //        else
    //        {
    //            card.GetComponent<InGameCard>().indexOnField = currentIndex;
    //            currentIndex++;
    //        }
    //    }
    //}

    public void ReCalculateServerCardIndexes()
    {
        int currentIndex = 0;
        foreach (GameObject card in monsterCards)
        {
            if(serverConfirmedCards.Contains(card))
            {
                card.GetComponent<InGameCard>().serverConfirmedIndex = currentIndex;
                currentIndex++;
            }
        }
    }

    public int RevertIndex(int index)
    {
        if (debugModeOn) Debug.Log("Reverting index: " + index);
        if (monsterCards.Count == 0) return 0;
        int ghostCardInt = 0;
        if (ghostCard != null) ghostCardInt = 1;
        if (debugModeOn) Debug.Log("GhostCard value: " + ghostCardInt + ", MonsterCards: " + monsterCards.Count + ", GameObjectName: " + gameObject.name);
        return monsterCards.Count - ghostCardInt - 1 - index;
    }

    public GameObject GetCardWithServerIndex(int index)
    {
        foreach (GameObject card in monsterCards)
        {
            if(card != ghostCard)
            {
                if (card.GetComponent<InGameCard>().serverConfirmedIndex == index) return card;
            }
            
        }

        Debug.LogError("Card was not found");
        return null;
    }

    public void RepositionMonsterCards()
    {
        cardXposDictionary = new Dictionary<GameObject, float>();

        Vector2 cardDim = (Vector2)References.i.fieldCard.GetComponent<BoxCollider>().size;
        float cardRowWidth = monsterCards.Count * cardDim.x + (monsterCards.Count - 1) * gapBetweenCards;
        float firstCardOffsetX = (-cardRowWidth + cardDim.x) / 2;
        float gapBetweenCardCenters = cardDim.x + gapBetweenCards;
        float newPosX;
        if (monsterCards.Count == 1) monsterCards[0].GetComponent<Transform>().localPosition = Vector3.zero;
        else 
        {
            for (int i = 0; i < monsterCards.Count; i++)
            {
                newPosX = firstCardOffsetX + gapBetweenCardCenters * i;
                Vector3 newPos = new Vector3(newPosX, 0, 0);
                if(ghostCard)
                {
                    if(monsterCards[i] != ghostCard)
                    {
                        cardXposDictionary.Add(monsterCards[i], newPosX);
                    }
                }
                else cardXposDictionary.Add(monsterCards[i], newPosX);
                monsterCards[i].GetComponent<CardMovement>().OnCardMove(newPos, GameManager.Instance.rearrangeDuration);
            }

        }
    }

    public void RecallCard(int player, GameObject card)
    {
        if(monsterCards.Contains(card))
        {
            if(player == GameManager.Instance.playerNumber)
            {
                Hand.AddNewCardToHand(card);
                unhandledCards.Remove(card);
                if (debugModeOn) Debug.Log("Removed unhandled");
                Destroy(card);
            }
            else
            {
                Debug.LogError("NOT IMPLEMENTED! DO IT!");
            }
            monsterCards.Remove(card);
        }
        
    }


    public void AttackWithCard(GameObject card)
    {
        if(gameObject.name == "YourMonsterZone")
        {
            for (int i = 0; i < monsterCards.Count; i++)
            {
                if (card == monsterCards[i])
                {
                    //animation here
                    //Vector3 defensiveCard;
                    ////check if there is a monstercard
                    //if (References.i.opponentMonsterZone.monsterCards.Count > 0) {
                    //    if (card.GetComponent<InGameCard>().cardData.attackDirection == Card.AttackDirection.Left) {
                    //        defensiveCard = References.i.opponentMonsterZone.monsterCards[monsterCards.Count - 1].transform.position;
                    //        Debug.Log("Attack to Left");
                    //    } else {
                    //        defensiveCard = References.i.opponentMonsterZone.monsterCards[0].transform.position;
                    //        Debug.Log("Attack to Right");
                    //    }
                        
                    //} else {
                    //    defensiveCard = GameObject.FindGameObjectWithTag("EnemyHand").transform.position;
                    //}

                    
                    WebSocketService.Attack(card.GetComponent<InGameCard>().serverConfirmedIndex);
                    if (debugModeOn) Debug.Log("found match for attacker: " + monsterCards[i].GetComponent<InGameCard>().cardData.cardName);
                    return;
                }
            }
            Debug.LogError("No card with this index on field!");
        }
        else
        {
            if (debugModeOn) Debug.Log("cannot attack with opponents card");
        }
    }


    public void UpdateCardData(bool isYourCard, PlayCardMessage playCardMessage)
    {
        if(isYourCard)
        {
            GameObject handledCard = unhandledCards[0];
            unhandledCards.Remove(handledCard);
            if (debugModeOn) Debug.Log("Removed unhandled " + unhandledCards.Count);
            serverConfirmedCards.Add(handledCard);
            ReCalculateServerCardIndexes();
            monsterCards[playCardMessage.boardIndex].GetComponent<InGameCard>().SetNewCardData(isYourCard, References.i.cardList.GetCardData(playCardMessage));
        }
        else
        {
            monsterCards[monsterCards.Count - 1 - playCardMessage.boardIndex].GetComponent<InGameCard>().SetNewCardData(isYourCard, References.i.cardList.GetCardData(playCardMessage));
        }

        
    }
    public void UpdateCardData(bool isYourCard, CardPowersMessage cardPower)
    {
        if (debugModeOn) Debug.Log("index " + cardPower.index);
        if(isYourCard)
        {
            GetCardWithServerIndex(cardPower.index).GetComponent<InGameCard>().SetStatLp(cardPower.lp);
            GetCardWithServerIndex(cardPower.index).GetComponent<InGameCard>().SetStatRp(cardPower.rp);
            
        }
        else
        {
            GetCardWithServerIndex(RevertIndex(cardPower.index)).GetComponent<InGameCard>().SetStatLp(cardPower.rp);
            GetCardWithServerIndex(RevertIndex(cardPower.index)).GetComponent<InGameCard>().SetStatRp(cardPower.lp);
            
        }
        GetCardWithServerIndex(cardPower.index).GetComponent<InGameCard>().UpdateCardTexts();
        if(isYourCard)
        {
            if (cardPower.lp <= 0 || cardPower.rp <= 0) RemoveMonsterCardNoDestroy(cardPower.index);
        }
        else if (cardPower.lp <= 0 || cardPower.rp <= 0) RemoveMonsterCardNoDestroy(RevertIndex(cardPower.index));
    }

    public int GetNewGhostCardIndex()
    {
        //calculate right index for card
        GameObject previousCard = null;
        Vector3 mousePos = Mouse.Instance.transform.position;
        foreach (GameObject card in cardXposDictionary.Keys)
        {
            if(cardXposDictionary[card] < mousePos.x)
            {
                if(previousCard == null)
                {
                    previousCard = card;
                }
                else if (Mathf.Abs(mousePos.x - card.transform.position.x) < Mathf.Abs(mousePos.x - previousCard.transform.position.x))
                {
                    previousCard = card;
                }
            }
        }
        if (previousCard == null) return 0;
        else
        {
            List<GameObject> monsterCardsNoGhostCards = new List<GameObject>(monsterCards);
            if(ghostCard) monsterCardsNoGhostCards.Remove(ghostCard);
            return monsterCardsNoGhostCards.IndexOf(previousCard) + 1;
        }

    }

    //Give this animation later
    public void MakeRoom()
    {
        int index = GetNewGhostCardIndex();
        if (!ghostCard)
        {
            GameObject newGhostCard = Instantiate(References.i.fieldCard, Mouse.Instance.mousePosInWorld, Quaternion.identity);
            newGhostCard.GetComponent<InGameCard>().ToggleGhostCard();
            newGhostCard.transform.SetParent(transform, true);
            monsterCards.Insert(index, newGhostCard);
            ghostCard = newGhostCard;
            RepositionMonsterCards();
        }
        else if(monsterCards[index] != ghostCard)
        {
            monsterCards.Remove(ghostCard);
            monsterCards.Insert(index, ghostCard);
            RepositionMonsterCards();
        }
        
    }

    public void RemoveGhostCard()
    {
        monsterCards.Remove(ghostCard);
        Destroy(ghostCard);
        RepositionMonsterCards();
    }
}
