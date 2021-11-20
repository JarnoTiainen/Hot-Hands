using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Hand : MonoBehaviour
{
    public CardList cardList;

    public static Hand Instance { get; private set; }

    private static List<GameObject> handCards = new List<GameObject>();
    private static List<GameObject> visibleHandCards = new List<GameObject>();
    private static List<GameObject> unhandledCards = new List<GameObject>();

    [SerializeField] private float gapBetweenCards = 0;
    [SerializeField] private float rotationSpeed = 0.1f;

    [SerializeField] private Vector2 handBoxDimension;

    public void Awake()
    {
        Instance = gameObject.GetComponent<Hand>();
    }

    private void Start()
    {
        Debug.Log("start");

        GameObject rightUpMark = Instantiate(References.i.testCube);
        rightUpMark.transform.SetParent(transform);
        rightUpMark.transform.localPosition = new Vector3(handBoxDimension.x, handBoxDimension.y, 0);

        GameObject leftUpMark = Instantiate(References.i.testCube);
        leftUpMark.transform.SetParent(transform);
        leftUpMark.transform.localPosition = new Vector3(-handBoxDimension.x, handBoxDimension.y, 0);

        GameObject rightDownMark = Instantiate(References.i.testCube);
        rightDownMark.transform.SetParent(transform);
        rightDownMark.transform.localPosition = new Vector3(handBoxDimension.x, -handBoxDimension.y, 0);

        GameObject leftDownMark = Instantiate(References.i.testCube);
        leftDownMark.transform.SetParent(transform);
        leftDownMark.transform.localPosition = new Vector3(-handBoxDimension.x, -handBoxDimension.y, 0);

    }

    public bool CheckIfInsideHandBox(Vector2 pos)
    {
        Vector2 localPos = pos - (Vector2)transform.position;
        if (localPos.x < Mathf.Abs(handBoxDimension.x) && localPos.y < Mathf.Abs(handBoxDimension.y))
        {
            return true;
        }
        return false;
    }

    //Adds new facedown card to hand (ADD DRAW ANIMATION HERE)
    public static void AddNewCard()
    {
        GameObject newCard = InstantiateNewCard();
        newCard.GetComponent<InGameCard>().cardHidden = true;
        newCard.GetComponent<InGameCard>().interActable = false;
        //adds the card to list of cards that have not been 
        unhandledCards.Add(newCard);

        //adds to list which manages card positions in hand
        visibleHandCards.Add(newCard);

        //adds the card to total cards in hand
        handCards.Add(newCard);
        SetNewCardPositions();
        
    }

    public GameObject GetVisibleCardWithSeed(string seed)
    {
        foreach(GameObject card in visibleHandCards)
        {
            if (card.GetComponent<InGameCard>().GetData().seed == seed) return card;
        }
        return null;
    }

    public static void AddNewCardToHand(GameObject card)
    {
        GameObject newCard = InstantiateNewCard();
        visibleHandCards.Add(newCard);
        handCards.Add(newCard);
        
        newCard.GetComponent<InGameCard>().SetNewCardData(true, card.GetComponent<InGameCard>().GetData());
        newCard.GetComponent<CardMovement>().OnCardRotate(Quaternion.Euler(0, 0, 0), Instance.rotationSpeed);
        SetNewCardPositions();
        Instance.UpdateCanAffortCards();
    }

    public void UpdateCanAffortCards()
    {
        foreach(GameObject card in handCards)
        {
            if(GameManager.Instance.playerStats.playerBurnValue >= card.GetComponent<InGameCard>().GetData().cost)
            {
                card.GetComponent<InGameCard>().ToggleCanAffordEffect(true);
            }
            else card.GetComponent<InGameCard>().ToggleCanAffordEffect(false);
        }
    }


    //instanciate and rotate card facedown
    private static GameObject InstantiateNewCard()
    {
        GameObject newCard = Instantiate(References.i.fieldCard, References.i.yourDeckObj.transform.position, Quaternion.Euler(0, 180, 0));
        newCard.transform.SetParent(Instance.gameObject.transform, true);
        
        return newCard;
    }

    [Button] public static GameObject RevealNewCard(DrawCardMessage drawCardMessage)
    {
        CardData cardData = Instance.cardList.GetCardData(drawCardMessage);
        GameObject card = null;
        if(cardData != null)
        {
            unhandledCards[0].GetComponent<InGameCard>().cardHidden = false;
            unhandledCards[0].GetComponent<InGameCard>().SetNewCardData(true, cardData);
            unhandledCards[0].GetComponent<CardMovement>().OnCardRotate(Quaternion.Euler(0,0,0), Instance.rotationSpeed);
            unhandledCards[0].GetComponent<InGameCard>().nameText.text = drawCardMessage.cardName;
            unhandledCards[0].GetComponent<InGameCard>().SetDescription();
            unhandledCards[0].GetComponent<InGameCard>().SetAttackDirectionSymbol();
            card = unhandledCards[0];
            unhandledCards.Remove(unhandledCards[0]);
            Instance.UpdateCanAffortCards();

        }

        return card;
    }
    public static void RemoveHiddenCard()
    {
        if(unhandledCards.Count > 0)
        {
            GameObject removedCard = unhandledCards[0];
            handCards.Remove(removedCard);
            visibleHandCards.Remove(removedCard);
            unhandledCards.Remove(removedCard);
            //Display effect here
            Destroy(removedCard);
            SetNewCardPositions();
        }
    }

    //Removes one card from hand and if there is no parameter remove card with last index
    [Button] public void TryRemoveCard(string seed)
    {
        GameObject removedCard = null;
        foreach (GameObject card in handCards)
        {
            if (card.GetComponent<InGameCard>().GetData().seed == seed) removedCard = card;
        }
        if(removedCard != null)
        {
            handCards.Remove(removedCard);
            visibleHandCards.Remove(removedCard);
            //GameManager.Instance.RemoveCardFromInGameCards(removedCard);
            Destroy(removedCard);
            SetNewCardPositions();
            Instance.UpdateCanAffortCards();
        }
        else
        {
            Debug.LogError("Card with seed: " + seed + " was not from hand cards!");
        }
    }

    public GameObject GetHandCardWithSeed(string seed)
    {
        foreach(GameObject card in handCards)
        {
            if(card.GetComponent<InGameCard>().GetData().seed == seed)
            {
                return card;
            }
        }
        return null;
    }

    [Button]
    public void RemoveCardNoDestroy(string seed)
    {
        GameObject removedCard = GetHandCardWithSeed(seed);
        handCards.Remove(removedCard);
        visibleHandCards.Remove(removedCard);
        SetNewCardPositions();
        Instance.UpdateCanAffortCards();
    }



    //sets new card positions in hand
    private static void SetNewCardPositions()
    {
        float inGameWidth = References.i.fieldCard.GetComponent<BoxCollider>().size.x;
        float totalCardsWidth = inGameWidth * visibleHandCards.Count + Instance.gapBetweenCards * (visibleHandCards.Count - 1);
        float newPosX;
        float firstCardOffsetX = (-totalCardsWidth + inGameWidth) / 2;
        float gapBetweenCardCenters = inGameWidth + Instance.gapBetweenCards;
        
        for (int i = 0; i < visibleHandCards.Count; i++)
        {   
            newPosX = firstCardOffsetX + gapBetweenCardCenters * i;
            Vector3 newPos = new Vector3(newPosX, 0, 0);
            visibleHandCards[i].GetComponent<CardMovement>().OnCardMove(newPos, GameManager.Instance.rearrangeDuration);
        }
    }

    //this function removes visible cards but leaves the card in hand cards list in case the cards need to be returned to hand
    public void RemoveVisibleCard(GameObject card)
    {
        visibleHandCards.Remove(card);
        SetNewCardPositions();
    }

    //returns card back to hand
    public void ReturnVisibleCard(GameObject card)
    {
        Debug.Log("name. " + card.name);
        card.GetComponent<BoxCollider>().enabled = true;
        card.transform.SetParent(transform, true);
        visibleHandCards.Add(card);
        SetNewCardPositions();
    }



    public int GetCardIndex(GameObject card)
    {
        for(int i = 0; i < handCards.Count; i++)
        {
            if (handCards[i] == card) return i;
        }
        Debug.LogError("Picked up card was not found from hand card list");
        return -1;
    }

    public CardData GetCardData(int handIndex)
    {
        return handCards[handIndex].GetComponent<InGameCard>().GetData();
    }
}
