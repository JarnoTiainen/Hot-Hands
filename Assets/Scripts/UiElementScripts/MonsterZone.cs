using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MonsterZone : MonoBehaviour
{
    public List<GameObject> monsterCards = new List<GameObject>();
    Dictionary<GameObject, float> cardXposDictionary = new Dictionary<GameObject, float>();
    
    [SerializeField] private float gapBetweenCards;
    [SerializeField] private float moveSpeed = 1;

    [SerializeField] private bool debugModeOn = false;
    public bool isYourMonsterZone;
    [SerializeField] [ShowIf("debugModeOn", true)]public GameObject ghostCard;


    private void Update()
    {
        if (Mouse.Instance.heldCard != null && isYourMonsterZone)
        {
            MakeRoom();
        }
    }

    [Button]public void AddNewMonsterCard(bool isYourCard, int boardIndex, CardData data)
    {
        if(debugModeOn)
        {
            if (isYourCard) Debug.Log("(MonsterZone) board Index: " + ghostCard.GetComponent<InGameCard>().indexOnField);
            else Debug.Log("(MonsterZone) board Index: " + (monsterCards.Count - 1 - boardIndex));
        }
        

        
        if(isYourCard)
        {
            ghostCard.GetComponent<InGameCard>().ToggleGhostCard();
            ghostCard.GetComponent<InGameCard>().SetNewCardData(isYourCard, data);
            ghostCard = null;
            ReCalculateCardIndexes();
        } else {
            
            //enemycard
            int index;
            if (monsterCards.Count == 0) {
                index = 0;
            } else {
                index = monsterCards.Count - boardIndex;
            }
            
            GameObject newMonster = Instantiate(References.i.fieldCard);
            newMonster.transform.SetParent(transform);
            monsterCards.Insert(index, newMonster);
            EnemyHand.Instance.RemoveCard(0);
            RepositionMonsterCards();
            newMonster.GetComponent<InGameCard>().SetNewCardData(isYourCard, data);
        }
    }

    public void RemoveMonsterCard(int index)
    {
        GameObject deadMonster = GetCardWithIndex(index);
        monsterCards.Remove(deadMonster);
        GameObject.Destroy(deadMonster);
        ReCalculateCardIndexes();
        RepositionMonsterCards();

    }

    public void ReCalculateCardIndexes()
    {
        int currentIndex = 0;
        foreach(GameObject card in monsterCards)
        {
            if(ghostCard)
            {
                if(card != ghostCard)
                {
                    card.GetComponent<InGameCard>().indexOnField = currentIndex;
                    currentIndex++;
                }
            }
            else
            {
                card.GetComponent<InGameCard>().indexOnField = currentIndex;
                currentIndex++;
            }
        }
    }

    public GameObject GetCardWithIndex(int index)
    {
        foreach(GameObject card in monsterCards)
        {
            if (card.GetComponent<InGameCard>().indexOnField == index) return card;
        }

        Debug.LogError("Card was not found");
        return null;
    }

    public void RemoveEnemyMonsterCard(int index)
    {
        GameObject deadMonster = monsterCards[monsterCards.Count - 1 - index];
        monsterCards.Remove(deadMonster);
        GameObject.Destroy(deadMonster);
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

    public void AttackWithCard(GameObject card)
    {
        if(gameObject.name == "YourMonsterZone")
        {
            for (int i = 0; i < monsterCards.Count; i++)
            {
                if (card == monsterCards[i])
                {
                    Debug.Log("found match for attacker: " + monsterCards[i].GetComponent<InGameCard>().cardData.cardName);
                    WebSocketService.Attack(i);
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
        //for now updates the card data of last played card

        if(debugModeOn)
        {
            if (isYourCard) Debug.Log("(MonsterZone) card index: " + playCardMessage.boardIndex);
            else Debug.Log("(MonsterZone) card index: " + (monsterCards.Count - 1 - playCardMessage.boardIndex));
        }

        if(isYourCard)
        {
            monsterCards[playCardMessage.boardIndex].GetComponent<InGameCard>().SetNewCardData(isYourCard, References.i.cardList.GetCardData(playCardMessage));
        }
        else
        {
            monsterCards[monsterCards.Count - 1 - playCardMessage.boardIndex].GetComponent<InGameCard>().SetNewCardData(isYourCard, References.i.cardList.GetCardData(playCardMessage));
        }

        
    }
    public void UpdateCardData(bool isYourCard, CardPowersMessage cardPower)
    {
        if(isYourCard)
        {
            monsterCards[cardPower.index].GetComponent<InGameCard>().SetStatLp(cardPower.lp);
            monsterCards[cardPower.index].GetComponent<InGameCard>().SetStatRp(cardPower.rp);
        }
        else
        {
            monsterCards[monsterCards.Count - 1 - cardPower.index].GetComponent<InGameCard>().SetStatLp(cardPower.rp);
            monsterCards[monsterCards.Count - 1 - cardPower.index].GetComponent<InGameCard>().SetStatRp(cardPower.lp);
        }
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
            return previousCard.GetComponent<InGameCard>().indexOnField + 1;
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
            newGhostCard.GetComponent<InGameCard>().indexOnField = index;
            newGhostCard.transform.SetParent(transform, true);
            monsterCards.Insert(index, newGhostCard);
            ghostCard = newGhostCard;
            RepositionMonsterCards();
        }
        else if(monsterCards[index] != ghostCard)
        {
            monsterCards.Remove(ghostCard);
            monsterCards.Insert(index, ghostCard);
            ghostCard.GetComponent<InGameCard>().indexOnField = index;
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
