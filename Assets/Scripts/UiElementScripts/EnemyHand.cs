using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyHand : MonoBehaviour
{
    public static EnemyHand Instance { get; private set; }
    private static List<GameObject> unhandledCards = new List<GameObject>();
    [SerializeField] private float gapBetweenCards = 0;
    [SerializeField] private float cardScaleInHand = 1;
    [SerializeField] private float moveSpeed = 0.5f;



    public void Awake()
    {
        Instance = gameObject.GetComponent<EnemyHand>();
        Debug.Log(gameObject.name);
    }

    private static GameObject InstantiateNewCard()
    {
        GameObject newCard = Instantiate(References.i.handCard, References.i.opponentDeckObj.transform.position, Quaternion.Euler(0, 180, 0));
        newCard.GetComponent<InGameCard>().cardHidden = true;
        newCard.transform.SetParent(Instance.gameObject.transform, true);
        newCard.transform.localScale = new Vector3(Instance.cardScaleInHand, Instance.cardScaleInHand, Instance.cardScaleInHand);
        return newCard;
    }

    //sets new card positions in hand
    private static void SetNewCardPositions()
    {
        float inGameWidth = References.i.handCard.GetComponent<BoxCollider>().size.x;
        float totalCardsWidth = inGameWidth * unhandledCards.Count + Instance.gapBetweenCards * (unhandledCards.Count - 1);
        float newPosX;
        float firstCardOffsetX = (-totalCardsWidth + inGameWidth) / 2;
        float gapBetweenCardCenters = inGameWidth + Instance.gapBetweenCards;
        
        for (int i = 0; i < unhandledCards.Count; i++)
        {   
            newPosX = firstCardOffsetX + gapBetweenCardCenters * i;
            Vector3 newPos = new Vector3(newPosX, 0, 0);
            unhandledCards[i].GetComponent<CardMovement>().OnCardMove(newPos, Instance.moveSpeed);
        }
    }

    //add a new card to enemy hand
    [Button] public static void AddNewCard()
    {
        GameObject newCard = InstantiateNewCard();
        unhandledCards.Add(newCard);
        SetNewCardPositions();
    }

    //Removes one card from enemy hand and if there is no parameter remove card with last index
    [Button] public void RemoveCard(int CardIndex = 0)
    {
        GameObject removedCard = unhandledCards[CardIndex];
        unhandledCards.Remove(removedCard);
        Destroy(removedCard);
        SetNewCardPositions();
    }
}
