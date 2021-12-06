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

    public void Start()
    {
        unhandledCards = new List<GameObject>();
    }

    public void Awake()
    {
        Instance = gameObject.GetComponent<EnemyHand>();
    }

    private static GameObject InstantiateNewCard()
    {
        GameObject newCard = Instantiate(References.i.fieldCard, References.i.opponentDeckObj.transform.position, Quaternion.Euler(0, 180, 0));
        newCard.GetComponent<InGameCard>().cardHidden = true;
        newCard.transform.SetParent(Instance.gameObject.transform, true);
        newCard.transform.localScale = new Vector3(Instance.cardScaleInHand, Instance.cardScaleInHand, Instance.cardScaleInHand);
        return newCard;
    }

    //sets new card positions in hand
    private static void SetNewCardPositions()
    {
        float inGameWidth = References.i.fieldCard.GetComponent<BoxCollider>().size.x;
        float totalCardsWidth = inGameWidth * unhandledCards.Count + Instance.gapBetweenCards * (unhandledCards.Count - 1);
        float newPosX;
        float firstCardOffsetX = (-totalCardsWidth + inGameWidth) / 2;
        float gapBetweenCardCenters = inGameWidth + Instance.gapBetweenCards;
        
        for (int i = 0; i < unhandledCards.Count; i++)
        {   
            newPosX = firstCardOffsetX + gapBetweenCardCenters * i;
            Vector3 newPos = new Vector3(newPosX, 0, 0);
            unhandledCards[i].GetComponent<CardMovement>().OnCardMove(newPos, GameManager.Instance.moveDuration);
            unhandledCards[i].GetComponent<CardMovement>().OnCardRotate(Quaternion.Euler(0, 180, 0), GameManager.Instance.rotationSpeed);
        }
    }

    //add a new card to enemy hand
    [Button] public static void AddNewCard(string seed, DrawCardMessage drawCardMessage = null)
    {
        GameObject newCard;
        if(References.i.mouse.tutorialMode) {
            newCard = References.i.opponentDeck.GetComponent<TutorialDeck>().TakeTopCard();
        } else {
            newCard = References.i.opponentDeck.GetComponent<DeckGaObConstructor>().TakeTopCard();
        }

        newCard.transform.SetParent(Instance.transform);
        unhandledCards.Add(newCard);
        CardData newCardData;
        if (drawCardMessage == null) {
            newCardData = newCard.GetComponent<InGameCard>().GetData();
        } else {
            newCardData = Hand.Instance.cardList.GetCardData(drawCardMessage);;
        }
        
        Debug.Log("enemy card data from enemy hand " + newCardData.cardName);

        newCardData.seed = seed;
        newCard.GetComponent<InGameCard>().SetNewCardData(false, newCardData);
        SetNewCardPositions();
        GameManager.Instance.AddCardToInGameCards(newCard);
        
    }

    [Button]
    public static void AddNewCardFromEnemyDeck(string seed, DrawCardMessage drawCardMessage = null)
    {
        GameObject newCard;
        if (References.i.mouse.tutorialMode)
        {
            newCard = References.i.yourDeck.GetComponent<TutorialDeck>().TakeTopCard();
        }
        else
        {
            newCard = References.i.yourDeck.GetComponent<DeckGaObConstructor>().TakeTopCard();
        }

        newCard.transform.SetParent(Instance.transform);
        unhandledCards.Add(newCard);
        CardData newCardData;
        if (drawCardMessage == null)
        {
            newCardData = newCard.GetComponent<InGameCard>().GetData();
        }
        else
        {
            newCardData = Hand.Instance.cardList.GetCardData(drawCardMessage); ;
        }

        Debug.Log("enemy card data from enemy hand " + newCardData.cardName);

        newCardData.seed = seed;
        newCard.GetComponent<InGameCard>().SetNewCardData(false, newCardData);
        SetNewCardPositions();
        GameManager.Instance.AddCardToInGameCards(newCard);

    }


    public GameObject GetCardWithSeed(string seed)
    {
        foreach(GameObject card in unhandledCards)
        {
            if(card.GetComponent<InGameCard>().GetData().seed == seed)
            {
                return card;
            }
        }
        return null;
    }


    //Removes one card from enemy hand and if there is no parameter remove card with last index
    [Button] public void RemoveCard(string seed)
    {
        GameObject removedCard = GetCardWithSeed(seed);
        if(removedCard != null)
        {
            unhandledCards.Remove(removedCard);
            Destroy(removedCard);
            SetNewCardPositions();
        }
        else
        {
            Debug.LogError("no card with seed found");
        }
    }
}
