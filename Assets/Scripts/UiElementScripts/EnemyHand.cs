using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyHand : MonoBehaviour
{
    public static EnemyHand Instance { get; private set; }
    private static List<GameObject> unhandledCards = new List<GameObject>();
    [SerializeField] private GameObject cardBase;
    private GameObject container;
    [SerializeField] private GameObject enemyDeckObj;
    [SerializeField] private float gapBetweenCards = 0;
    [SerializeField] private float cardScaleInHand = 1;
    [SerializeField] private float moveSpeed = 0.5f;



    public void Awake()
    {
        Instance = gameObject.GetComponent<EnemyHand>();
        container = gameObject;
        enemyDeckObj = GameObject.FindGameObjectWithTag("EnemyDeck");
        Debug.Log(gameObject.name);
    }

    private static GameObject InstantiateNewCard()
    {
        GameObject newCard = Instantiate(Instance.cardBase, Instance.enemyDeckObj.transform.position, Quaternion.Euler(0, 180, 0));
        newCard.transform.SetParent(Instance.container.transform, true);
        newCard.transform.localScale = new Vector3(Instance.cardScaleInHand, Instance.cardScaleInHand, Instance.cardScaleInHand);
        return newCard;
    }

    //sets new card positions in hand
    private static void SetNewCardPositions()
    {
        float inGameWidth = Instance.cardBase.transform.GetChild(0).GetComponent<BoxCollider>().size.x;
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

    private static float TotalCardsWidth()
    {
        float totalCardWidth = 0;
        foreach (GameObject card in unhandledCards)
        {
            totalCardWidth += Instance.cardBase.transform.GetChild(0).GetComponent<BoxCollider>().size.x;
            if (card != unhandledCards[0]) totalCardWidth += Instance.gapBetweenCards;
        }

        return totalCardWidth;
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
        Debug.LogWarning("REMOVAL!");
        GameObject removedCard = unhandledCards[CardIndex];
        unhandledCards.Remove(removedCard);
        Destroy(removedCard);
        SetNewCardPositions();
    }
}
