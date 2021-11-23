using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MonsterZone : MonoBehaviour
{
    public List<GameObject> unhandledCards = new List<GameObject>();
    public List<GameObject> monsterCards = new List<GameObject>();
    Dictionary<GameObject, float> cardXposDictionary = new Dictionary<GameObject, float>();
    [SerializeField] private float gapBetweenCards;
    [SerializeField] private bool debugModeOn = false;
    public bool isYourMonsterZone;
    [SerializeField] [ShowIf("debugModeOn", true)]public GameObject ghostCard = null;

    private void Update()
    {
        if (Mouse.Instance.heldCard != null && isYourMonsterZone && GameManager.Instance.playerStats.playerFieldCards < GameManager.Instance.maxFieldCardCount)
        {
            if(Mouse.Instance.heldCard.GetComponent<InGameCard>().GetData().cardType == Card.CardType.Monster)
            {
                MakeRoom();
            } else {
                RemoveGhostCard();
            }
        } else {
            RemoveGhostCard();
        }
    } 

    public GameObject AutoAddNewMonsterCard(bool isYourCard, int boardIndex, CardData data)
    {
        GameObject newMonster = Instantiate(References.i.fieldCard);

        if (debugModeOn) Debug.Log("index: " + boardIndex);
        monsterCards.Insert(boardIndex, newMonster);
        
        newMonster.transform.SetParent(transform, true);
        Vector3 instancePos = CalculatePosition(boardIndex, !isYourCard, newMonster);
        newMonster.transform.position = instancePos;
        newMonster.GetComponent<InGameCard>().SetNewCardData(isYourCard, data);
        newMonster.GetComponent<InGameCard>().SetDescription();
        newMonster.GetComponent<InGameCard>().SetAttackDirectionSymbol();
        newMonster.GetComponent<HandCard>().targetable = false;
        RepositionMonsterCards();
        return newMonster;
    }

   
    [Button]public GameObject AddNewMonsterCard(bool isYourCard, int boardIndex, CardData data, bool isPreAddedCard = true)
    {
        GameObject newCard;
        if(isYourCard)
        {
            ghostCard.GetComponent<InGameCard>().ToggleGhostCard(false);
            ghostCard.GetComponent<InGameCard>().SetNewCardData(isYourCard, data);
            ghostCard.GetComponent<InGameCard>().SetDescription();
            ghostCard.GetComponent<InGameCard>().SetAttackDirectionSymbol();
            ghostCard.GetComponent<HandCard>().targetable = false;
            unhandledCards.Add(ghostCard);
            if(debugModeOn) Debug.Log("Added unhandled");
            newCard = ghostCard;
            newCard.transform.position = CalculatePosition(boardIndex, isYourCard, newCard);
            ghostCard = null;

        } 
        else 
        {

            //enemycard
            int index;
            if (monsterCards.Count == 0) {
                index = 0;
            } else {
                index = monsterCards.Count - boardIndex;
            }

            GameObject newMonster = Instantiate(References.i.fieldCard);

            newMonster.transform.SetParent(transform);
            
            if (debugModeOn) Debug.Log("index: " + index);
            if (index > monsterCards.Count) index = monsterCards.Count;
            monsterCards.Insert(index, newMonster);
            newMonster.transform.position = CalculatePosition(boardIndex, isYourCard, newMonster);
            newMonster.GetComponent<InGameCard>().SetNewCardData(isYourCard, data);
            if(GameManager.Instance.playerNumber == 0)
            {
                newMonster.GetComponent<InGameCard>().owner = 1;
            }
            else
            {
                newMonster.GetComponent<InGameCard>().owner = 0;
            }
            newMonster.GetComponent<InGameCard>().SetDescription();
            newMonster.GetComponent<InGameCard>().SetAttackDirectionSymbol();
            newMonster.GetComponent<HandCard>().targetable = false;
            GameManager.Instance.AddCardToInGameCards(newMonster);
            RepositionMonsterCards();
            newCard = newMonster;
            
        }
        //RepositionMonsterCards();
        return newCard;
    }

    /// <summary>
    /// Calculate the instantiation position for new card
    /// </summary>
    /// <param name="boardIndex"></param>
    /// <param name="isYourCard"></param>
    /// <returns></returns>
    private Vector3 CalculatePosition(int boardIndex, bool isYourCard, GameObject cardObj)
    {
        cardXposDictionary = new Dictionary<GameObject, float>();

        Vector3 newPos;
        Vector2 cardDim = (Vector2)References.i.fieldCard.GetComponent<BoxCollider>().size;
        float cardRowWidth = monsterCards.Count * cardDim.x + (monsterCards.Count - 1) * gapBetweenCards;
        float firstCardOffsetX = (-cardRowWidth + cardDim.x) / 2;
        float gapBetweenCardCenters = cardDim.x + gapBetweenCards;
        float newPosX;
        for (int i = 0; i < monsterCards.Count; i++) {
            if(monsterCards[i] == cardObj) {
                newPosX = firstCardOffsetX + gapBetweenCardCenters * i;
                if (isYourCard) {
                    newPos = new Vector3(newPosX, References.i.yourMonsterZone.transform.position.y, References.i.yourMonsterZone.transform.position.z);
                } else {
                    newPos = new Vector3(newPosX, References.i.opponentMonsterZone.transform.position.y, References.i.opponentMonsterZone.transform.position.z);
                }

                Debug.Log("index is " + boardIndex);
                    
                return newPos;
            }
        }
        return Vector3.zero;
    }


    public void TryRemoveMonsterCard(string seed)
    {
        if(monsterCards.Contains(GameManager.Instance.GetCardFromInGameCards(seed)))
        {
            if (debugModeOn) Debug.Log("Removing monster " + seed);
            GameObject deadMonster = GetCardWithSeed(seed);
            monsterCards.Remove(deadMonster);
            deadMonster.GetComponent<InGameCard>().StartDestructionEvent();
            RepositionMonsterCards();
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

    public GameObject GetCardWithSeed(string seed)
    {
        foreach (GameObject card in monsterCards)
        {
            if(card != ghostCard)
            {
                if (card.GetComponent<InGameCard>().GetData().seed == seed) return card;
            }
            
        }

        Debug.LogError("Card was not found");
        return null;
    }

    [Button] public void RepositionMonsterCards()
    {
        Debug.Log("Repositioning monster cards " + monsterCards.Count);
        
        cardXposDictionary = new Dictionary<GameObject, float>();

        Vector2 cardDim = (Vector2)References.i.fieldCard.GetComponent<BoxCollider>().size;
        float cardRowWidth = monsterCards.Count * cardDim.x + (monsterCards.Count - 1) * gapBetweenCards;
        float firstCardOffsetX = (-cardRowWidth + cardDim.x) / 2;
        float gapBetweenCardCenters = cardDim.x + gapBetweenCards;
        float newPosX;
        if (monsterCards.Count == 1) {
            monsterCards[0].GetComponent<Transform>().localPosition = Vector3.zero;
        } else {
            for (int i = 0; i < monsterCards.Count; i++) {
                newPosX = firstCardOffsetX + gapBetweenCardCenters * i;
                Vector3 newPos = new Vector3(newPosX, 0, transform.position.z);
                if (ghostCard) {
                    if (monsterCards[i] != ghostCard) {
                        cardXposDictionary.Add(monsterCards[i], newPosX);
                    }
                } else cardXposDictionary.Add(monsterCards[i], newPosX);

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

                    
                    WebSocketService.Attack(card.GetComponent<InGameCard>().GetData().seed);
                    if (debugModeOn) Debug.Log("found match for attacker: " + monsterCards[i].GetComponent<InGameCard>().GetData().cardName);
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
            if(monsterCards.IndexOf(handledCard) != playCardMessage.boardIndex)
            {
                monsterCards.Remove(handledCard);
                monsterCards.Insert(playCardMessage.boardIndex, handledCard);
            }


            unhandledCards.Remove(handledCard);
            if (debugModeOn) Debug.Log("Removed unhandled " + unhandledCards.Count);
            Debug.Log("card name in slot " + monsterCards[playCardMessage.boardIndex].GetComponent<InGameCard>().GetData().cardName);
            monsterCards[playCardMessage.boardIndex].GetComponent<InGameCard>().SetNewCardData(isYourCard, References.i.cardList.GetCardData(playCardMessage));
        }
        else
        {
            monsterCards[monsterCards.Count - 1 - playCardMessage.boardIndex].GetComponent<InGameCard>().SetNewCardData(isYourCard, References.i.cardList.GetCardData(playCardMessage));
        }
    }
    public GameObject UpdateCardData(bool isYourCard, SummonCardMessage summonCardMessage)
    {
        GameObject handledCard;

        if (isYourCard)
        {
            handledCard = unhandledCards[0];
            if (monsterCards.IndexOf(handledCard) != summonCardMessage.boardIndex)
            {
                Debug.Log("chost index " + summonCardMessage.boardIndex);
                monsterCards.Remove(handledCard);
                monsterCards.Insert(summonCardMessage.boardIndex, handledCard);
            }
            RepositionMonsterCards();

            unhandledCards.Remove(handledCard);
            if (debugModeOn) Debug.Log("Removed unhandled " + unhandledCards.Count);
            handledCard.GetComponent<InGameCard>().SetNewCardData(isYourCard, References.i.cardList.GetCardData(summonCardMessage));
        }
        else
        {
            monsterCards[monsterCards.Count - 1 - summonCardMessage.boardIndex].GetComponent<InGameCard>().SetNewCardData(isYourCard, References.i.cardList.GetCardData(summonCardMessage));
            handledCard = monsterCards[monsterCards.Count - 1 - summonCardMessage.boardIndex];
        }
        return handledCard;
    }

    public void UpdateCardData(bool isYourCard, CardPowersMessage cardPower)
    {
        if (debugModeOn) Debug.Log("seed " + cardPower.seed);
        if(isYourCard)
        {
            GetCardWithSeed(cardPower.seed).GetComponent<InGameCard>().UpdateRPLP(cardPower.rp, cardPower.lp);
            
        }
        else
        {
            GetCardWithSeed(cardPower.seed).GetComponent<InGameCard>().UpdateRPLP(cardPower.lp, cardPower.rp);

        }
        GetCardWithSeed(cardPower.seed).GetComponent<InGameCard>().UpdateCardTexts();
        if (cardPower.lp <= 0 || cardPower.rp <= 0) GameManager.Instance.GetCardFromInGameCards(cardPower.seed).GetComponent<InGameCard>().interActable = false;
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
            newGhostCard.GetComponent<InGameCard>().ToggleGhostCard(true);
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
        if (monsterCards.Contains(ghostCard)) {
            monsterCards.Remove(ghostCard);
            Destroy(ghostCard);
            RepositionMonsterCards();
        }
        
        
    }
}
