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
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private bool debugModeOn = false;
    public bool isYourMonsterZone;
    [SerializeField] [ShowIf("debugModeOn", true)]public GameObject ghostCard;


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
            Debug.Log("Added unhandled");
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
            Debug.Log("index: " + index);
            monsterCards.Insert(index, newMonster);
            newMonster.GetComponent<InGameCard>().SetNewCardData(isYourCard, data);
            ReCalculateServerCardIndexes();
            RepositionMonsterCards();
        }
    }

    public void RemoveMonsterCard(int index)
    {
        Debug.Log("Removing monster " + index);
        GameObject deadMonster = GetCardWithServerIndex(index);
        monsterCards.Remove(deadMonster);
        serverConfirmedCards.Remove(deadMonster);
        GameObject.Destroy(deadMonster);
        ReCalculateServerCardIndexes();
        RepositionMonsterCards();

    }
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
        if (monsterCards.Count == 0) return 0;
        int ghostCardInt = 0;
        if (ghostCard != null) ghostCardInt = 1;
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

    public void RemoveEnemyMonsterCard(int index)
    {
        GameObject deadMonster = GetCardWithServerIndex(RevertIndex(index));
        monsterCards.Remove(deadMonster);
        GameObject.Destroy(deadMonster);
        ReCalculateServerCardIndexes();
        RepositionMonsterCards();
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
                monsterCards[i].GetComponent<CardMovement>().OnCardMove(newPos, moveSpeed);
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
                Debug.Log("Removed unhandled");
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
                    WebSocketService.Attack(card.GetComponent<InGameCard>().serverConfirmedIndex);
                    return;
                }
            }
            Debug.LogError("No card with this index on field!");
        }
        else
        {
            Debug.Log("cannot attack with opponents card");
        }
    }


    public void UpdateCardData(bool isYourCard, PlayCardMessage playCardMessage)
    {
        if(isYourCard)
        {
            GameObject handledCard = unhandledCards[0];
            unhandledCards.Remove(handledCard);
            Debug.Log("Removed unhandled " + unhandledCards.Count);
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
        Debug.Log("index " + cardPower.index);
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
    }

    public int GetNewGhostCardIndex()
    {
        //calculate right index for card
        GameObject previousCard = null;
        Vector3 mousePos = Mouse.Instance.transform.position;
        Debug.Log("keys: " + cardXposDictionary.Keys.Count);
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
        Debug.Log("new ghost card index " + index);
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
