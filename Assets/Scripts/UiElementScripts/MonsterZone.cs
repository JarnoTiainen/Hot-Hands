using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MonsterZone : MonoBehaviour
{
    [SerializeField] private CardList cardList;
    public List<GameObject> monsterCards = new List<GameObject>();
    [SerializeField] private Mouse mouse;
    [SerializeField] private GameObject refCard;
    [SerializeField] private GameObject testCard;
    
    [SerializeField] private float makeRoomEffectDistance;
    [SerializeField] private float gapBetweenCards;
    [SerializeField] private float moveSpeed = 1;


    private void Awake()
    {

    }

    [Button]public void AddNewMonsterCard(CardData cardData)
    {
        GameObject newMonster = Instantiate(testCard, transform);
        //newMonster.GetComponent<InGameCard>().SetNewCardData(cardData);


        //FIX THIS ASAP
        monsterCards.Add(newMonster);
        RepositionMonsterCards();
    }

    public void RemoveMonsterCard(int index)
    {
        GameObject deadMonster = monsterCards[index];
        monsterCards.Remove(deadMonster);
        GameObject.Destroy(deadMonster);
        RepositionMonsterCards();
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
        Vector2 cardDim = (Vector2)refCard.GetComponent<BoxCollider>().size;
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
                //monsterCards[i].GetComponent<CardMovement>().OnCardMove(newPos, moveSpeed);
                monsterCards[i].GetComponent<Transform>().localPosition = newPos;
            }

        }
    }

    private void Update()
    {
        if(mouse.heldCard != null)
        {
            if (Mathf.Abs(mouse.heldCard.transform.position.y - transform.position.y) < makeRoomEffectDistance)
            {
                MakeRoom();
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

    public void UpdateCardData(DrawCardMessage drawCardMessage)
    {
        //for now updates the card data of last played card
        monsterCards[monsterCards.Count-1].GetComponent<InGameCard>().SetNewCardData(cardList.GetCardData(drawCardMessage));
    }
    public void UpdateCardData(int index, int lp, int rp)
    {
        monsterCards[index].GetComponent<InGameCard>().SetStatLp(lp);
        monsterCards[index].GetComponent<InGameCard>().SetStatRp(rp);
    }

    public void UpdateEnemyCardData(int index, int lp, int rp)
    {
        monsterCards[monsterCards.Count - 1 - index].GetComponent<InGameCard>().SetStatLp(lp);
        monsterCards[monsterCards.Count - 1 - index].GetComponent<InGameCard>().SetStatRp(rp);
    }


    //Give this animation later //???
    public void MakeRoom()
    {

    }
}
