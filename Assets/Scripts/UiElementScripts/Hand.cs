using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Hand : MonoBehaviour
{
    public CardList cardList;

    public static Hand Instance { get; private set; }

    private Dictionary<GameObject, GameObject> visibleHandCardPreviews = new Dictionary<GameObject, GameObject>();

    private static List<GameObject> handCards = new List<GameObject>();
    private static List<GameObject> visibleHandCards = new List<GameObject>();
    private static List<GameObject> unhandledCards = new List<GameObject>();


    [SerializeField] private GameObject handCard;
    [SerializeField] private float gapBetweenCards = 0;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float rotationSpeed = 0.1f;

    private Canvas uiCanvas;
    private Canvas canvas;
    private GameObject deckObj;

    public void Awake()
    {
        Instance = gameObject.GetComponent<Hand>();
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        deckObj = GameObject.FindGameObjectWithTag("Deck");
    }

    //Adds new facedown card to hand (ADD DRAW ANIMATION HERE)
    public static void AddNewCard()
    {
        GameObject newCard = InstantiateNewCard();
        newCard.GetComponent<InGameCard>().cardHidden = true;
        //adds the card to list of cards that have not been 
        unhandledCards.Add(newCard);

        //adds to list which manages card positions in hand
        visibleHandCards.Add(newCard);

        //adds the card to total cards in hand
        handCards.Add(newCard);
        SetNewCardPositions();
    }


    //instanciate and rotate card facedown
    private static GameObject InstantiateNewCard()
    {
        GameObject newCard = Instantiate(Instance.handCard, Instance.deckObj.transform.position, Quaternion.Euler(0, 180, 0));
        newCard.transform.SetParent(Instance.gameObject.transform, true);
        
        return newCard;
    }

    [Button] public static void RevealNewCard(DrawCardMessage drawCardMessage)
    {
        CardData cardData = Instance.cardList.GetCardData(drawCardMessage);
        if(cardData != null)
        {
            unhandledCards[0].GetComponent<InGameCard>().cardHidden = false;
            unhandledCards[0].GetComponent<InGameCard>().SetNewCardData(true, cardData);
            unhandledCards[0].GetComponent<CardMovement>().OnCardRotate(Quaternion.Euler(0,0,0), Instance.rotationSpeed);
            unhandledCards[0].GetComponent<InGameCard>().nameText.text = drawCardMessage.cardName;
            unhandledCards.Remove(unhandledCards[0]);

        }
    }



    //Removes one card from hand and if there is no parameter remove card with last index
    [Button] public void RemoveCard(int CardIndex = 0)
    {
        GameObject removedCard = handCards[CardIndex];
        handCards.Remove(removedCard);
        visibleHandCards.Remove(removedCard);
        Destroy(removedCard);
        SetNewCardPositions();
    }

    [Button]
    public void RemoveCardNoDestroy(int CardIndex = 0)
    {
        GameObject removedCard = handCards[CardIndex];
        handCards.Remove(removedCard);
        visibleHandCards.Remove(removedCard);
        SetNewCardPositions();
    }



    //sets new card positions in hand
    private static void SetNewCardPositions()
    {
        float inGameWidth = Instance.handCard.GetComponent<BoxCollider>().size.x;
        float totalCardsWidth = inGameWidth * visibleHandCards.Count + Instance.gapBetweenCards * (visibleHandCards.Count - 1);
        float newPosX;
        float firstCardOffsetX = (-totalCardsWidth + inGameWidth) / 2;
        float gapBetweenCardCenters = inGameWidth + Instance.gapBetweenCards;
        
        for (int i = 0; i < visibleHandCards.Count; i++)
        {   
            newPosX = firstCardOffsetX + gapBetweenCardCenters * i;
            Vector3 newPos = new Vector3(newPosX, 0, 0);
            visibleHandCards[i].GetComponent<CardMovement>().OnCardMove(newPos, Instance.moveSpeed);
        }
    }

    //this function removes visible cards but leaves the card in hand cards list in case the cards need to be returned to hand
    public void RemoveVisibleCard(GameObject card)
    {
        visibleHandCards.Remove(card);
        SetNewCardPositions();
    }

    //returns card back to hand
    public void ReturnVisibleCard(GameObject card, int handIndex)
    {
        card.GetComponent<BoxCollider>().enabled = true;
        card.transform.SetParent(transform, true);
        visibleHandCards.Insert(handIndex, card);
        SetNewCardPositions();
    }

    //shows card tooltip
    public void ShowCardTooltip(GameObject card)
    {
        if(!unhandledCards.Contains(card))
        {
            visibleHandCardPreviews.Add(card, UiCardPreviewManager.Instance.ShowCardPreview(card));
        }
        
    }

    //hides card tooltip
    public void HideCardTooltip(GameObject hoveredCard)
    {
        if(visibleHandCardPreviews.ContainsKey(hoveredCard))
        {
            GameObject preview = visibleHandCardPreviews[hoveredCard];
            UiCardPreviewManager.Instance.HideCardPreview(preview);
            visibleHandCardPreviews.Remove(hoveredCard);
        }
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
        return handCards[handIndex].GetComponent<InGameCard>().cardData;
    }
}
