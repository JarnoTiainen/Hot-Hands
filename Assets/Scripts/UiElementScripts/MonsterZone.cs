using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MonsterZone : MonoBehaviour
{
    public List<GameObject> monsterCards = new List<GameObject>();
    [SerializeField] private Mouse mouse;
    [SerializeField] private GameObject refCard;
    [SerializeField] private GameObject testCard;
    
    [SerializeField] private float makeRoomEffectDistance;
    [SerializeField] private float gapBetweenCards;

    private void Awake()
    {

    }

    [Button]public void AddNewMonsterCard(CardData cardData)
    {
        GameObject newMonster = Instantiate(testCard);
        newMonster.transform.SetParent(transform);
        //newMonster.GetComponent<InGameCard>().SetNewCardData(cardData);
        monsterCards.Add(newMonster);
        RepositionMonsterCards();
    }

    public void RemoveMonsterCard()
    {

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
                Vector2 newPos = new Vector2(newPosX, 0);
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


    //Give this animation later
    public void MakeRoom()
    {

    }
}
