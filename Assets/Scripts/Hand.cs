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


    [SerializeField] private GameObject cardBase;
    [SerializeField] private float gapBetweenCards;

    private Canvas uiCanvas;
    private Canvas canvas;

    


    public void Awake()
    {
        Instance = gameObject.GetComponent<Hand>();
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    //Adds new facedown card to hand (ADD DRAW ANIMATION HERE)
    public static void AddNewCard()
    {
        GameObject newCard = InstantiateNewCard();

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
        GameObject newCard = Instantiate(Instance.cardBase);
        newCard.transform.SetParent(Instance.gameObject.transform);
        newCard.transform.localPosition = new Vector3(0, 0, 0);
        newCard.transform.rotation = Quaternion.Euler(0, 180, 0);
        return newCard;
    }

    [Button] public static void RevealNewCard(DrawCardMessage drawCardMessage)
    {
        Card card = null;
        foreach (CardList.ListCard listCard in Instance.cardList.allCards)
        {
            if (listCard.name == drawCardMessage.cardName)
            {
                card = listCard.card;
            }
        }
        if (card != null)
        {
            CardData cardData = new CardData(card.cardSprite, drawCardMessage.cardName, drawCardMessage.cardCost, drawCardMessage.cardValue, (Card.CardType)drawCardMessage.cardType, drawCardMessage.rp, drawCardMessage.lp);
            unhandledCards[0].transform.GetChild(0).GetComponent<UiCardInHand>().cardData = cardData;
            unhandledCards[0].transform.rotation = Quaternion.Euler(0, 0, 0);
            unhandledCards[0].transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = drawCardMessage.cardName;
            unhandledCards.Remove(unhandledCards[0]);
            Debug.Log("cost " + cardData.cost);
            Debug.Log("value " + cardData.value);
        }
        else
        {
            Debug.LogError("Card with name: " + drawCardMessage.cardName + " was not found from Unity side databse");

            //REMOVE THESE WHEN SERVER CARD NAME THING IS FIXED
            unhandledCards[0].transform.rotation = Quaternion.Euler(0, 0, 0);
            unhandledCards[0].transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = drawCardMessage.cardName;
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

    //sets new card positions in hand
    private static void SetNewCardPositions()
    {
        float inGameWidth = Instance.cardBase.transform.GetChild(0).GetComponent<BoxCollider>().size.x;
        float totalCardsWidth = inGameWidth * visibleHandCards.Count + Instance.gapBetweenCards * (visibleHandCards.Count - 1);
        float newPosX;
        float firstCardOffsetX = (-totalCardsWidth + inGameWidth) / 2;
        float gapBetweenCardCenters = inGameWidth + Instance.gapBetweenCards;

        if (visibleHandCards.Count == 1) visibleHandCards[0].GetComponent<Transform>().localPosition = Vector3.zero;
        else
        {
            for (int i = 0; i < visibleHandCards.Count; i++)
            {
                newPosX = firstCardOffsetX + gapBetweenCardCenters * i;
                Vector2 newPos = new Vector2(newPosX, 0);
                visibleHandCards[i].GetComponent<Transform>().localPosition = newPos;
            }
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
        card.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        visibleHandCards.Add(card);
        SetNewCardPositions();
        
    }

    //shows card tooltip
    public void ShowCardTooltip(GameObject card)
    {
        if(!unhandledCards.Contains(card))
        {
            Vector2 uiCanvasDimensions = uiCanvas.GetComponent<RectTransform>().sizeDelta;
            Vector2 pos = (Vector2)Camera.main.WorldToScreenPoint(card.GetComponent<Transform>().position);
            Vector2 canvasDimensions = canvas.GetComponent<RectTransform>().sizeDelta;
            Vector2 rel = new Vector2(pos.x / canvasDimensions.x, pos.y / canvasDimensions.y);
            Vector2 posInUiCanvas = new Vector2(rel.x * uiCanvasDimensions.x, rel.y * uiCanvasDimensions.y) - uiCanvasDimensions / 2;
            visibleHandCardPreviews.Add(card, UiCardPreviewManager.Instance.ShowCardPreview(posInUiCanvas));
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
}
