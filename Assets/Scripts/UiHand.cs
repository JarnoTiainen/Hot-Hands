using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class UiHand : MonoBehaviour
{
    public CardList cardList;

    public static UiHand Instance { get; private set; }
    private Dictionary<GameObject, GameObject> visibleHandCardPreviews = new Dictionary<GameObject, GameObject>();
    [SerializeField] private static List<GameObject> handCards = new List<GameObject>();
    [SerializeField] private static List<GameObject> visibleHandCards = new List<GameObject>();
    [SerializeField] private GameObject cardBase;
    private GameObject container;
    [SerializeField] private float gapBetweenCards;
    [SerializeField] private float cardScaleInHand = 1;
    [SerializeField] private float hoveredCardScaleMultiplier;
    [SerializeField] private float hoveredCardLiftAmountY;
    [SerializeField] private float hoveredCardLiftAmountZ;
    [SerializeField] float scaleTransitionTime;
    private Canvas uiCanvas;
    private Canvas canvas;

    private static List<GameObject> unhandledCards = new List<GameObject>();


    public void Awake()
    {

        Instance = gameObject.GetComponent<UiHand>();
        container = gameObject;
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    [Button] public static void AddNewCard()
    {
        GameObject newCard = InstantiateNewCard();
        unhandledCards.Add(newCard);
        visibleHandCards.Add(newCard);
        handCards.Add(newCard);
        SetNewCardPositions();
    }

    [Button]public static void RevealNewCard(DrawCardMessage drawCardMessage)
    {
        Card card = null;
        foreach(CardList.ListCard listCard in Instance.cardList.allCards)
        {
            if(listCard.name == drawCardMessage.cardName)
            {
                card = listCard.card;
            }
        }
        if(card != null)
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
        }
    }
    [Button] public void RemoveCard(int CardIndex = 0)
    {
        GameObject removedCard = handCards[CardIndex];
        handCards.Remove(removedCard);
        visibleHandCards.Remove(removedCard);
        Destroy(removedCard);
        SetNewCardPositions();
    }

    private static GameObject InstantiateNewCard()
    {
        GameObject newCard = Instantiate(Instance.cardBase);
        newCard.transform.SetParent(Instance.container.transform);
        newCard.transform.localPosition = new Vector3(0,0,0);
        newCard.transform.localScale = new Vector3(Instance.cardScaleInHand, Instance.cardScaleInHand, Instance.cardScaleInHand);
        newCard.transform.rotation = Quaternion.Euler(0, 180, 0);
        return newCard;
    }

    private static void SetNewCardPositions()
    {

        for(int i = 0; i < visibleHandCards.Count; i++)
        {
            if(i == 0)
            {
                float inGameWidth = Instance.cardBase.transform.GetChild(0).GetComponent<BoxCollider>().size.x;
                float totalCardsWidth = TotalCardsWidth();
                float cardPosX = -totalCardsWidth / 2 + inGameWidth / 2;
                visibleHandCards[i].transform.localPosition = new Vector3(cardPosX, visibleHandCards[i].transform.localPosition.y, visibleHandCards[i].transform.localPosition.z);
            }
            else
            {
                float newPosX;
                float previousCardPosX = visibleHandCards[i - 1].transform.localPosition.x;
                newPosX = previousCardPosX;
                newPosX += Instance.cardBase.transform.GetChild(0).GetComponent<BoxCollider>().size.x * visibleHandCards[i - 1].transform.localScale.x / 2;
                newPosX += Instance.cardBase.transform.GetChild(0).GetComponent<BoxCollider>().size.x * visibleHandCards[i].transform.localScale.x / 2;
                newPosX += Instance.gapBetweenCards;

                visibleHandCards[i].transform.localPosition = new Vector3(newPosX, visibleHandCards[i].transform.localPosition.y, visibleHandCards[i].transform.localPosition.z);
            }
        }
    }

    private static float TotalCardsWidth()
    {
        float totalCardWidth = 0;
        foreach(GameObject card in visibleHandCards)
        {
            totalCardWidth += Instance.cardBase.transform.GetChild(0).GetComponent<BoxCollider>().size.x;
            if (card != visibleHandCards[0]) totalCardWidth += Instance.gapBetweenCards;
        }
        
        return totalCardWidth;
    }

    public void IncreaseCardSize(GameObject card)
    {
        card.transform.localPosition += new Vector3(0, hoveredCardLiftAmountY, hoveredCardLiftAmountZ);
        card.transform.localScale = new Vector3(hoveredCardScaleMultiplier, hoveredCardScaleMultiplier, hoveredCardScaleMultiplier);
        SetNewCardPositions();
    }

    public void DecreaseCardSize(GameObject card)
    {
        card.transform.localPosition -= new Vector3(0, hoveredCardLiftAmountY, hoveredCardLiftAmountZ);
        card.transform.localScale = new Vector3(cardScaleInHand, cardScaleInHand, cardScaleInHand);
        SetNewCardPositions();
    }

    public void RemoveVisibleCard(GameObject card)
    {
        visibleHandCards.Remove(card);
        SetNewCardPositions();
    }
    public void ReturnVisibleCard(GameObject card)
    {
        card.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        visibleHandCards.Add(card);
        SetNewCardPositions();
        
    }

    public void ShowCardTooltip(GameObject card)
    {
        if(!unhandledCards.Contains(card))
        {
            Debug.Log("Showing tooltip");
            Vector2 uiCanvasDimensions = uiCanvas.GetComponent<RectTransform>().sizeDelta;

            Vector2 pos = (Vector2)Camera.main.WorldToScreenPoint(card.GetComponent<Transform>().position);

            Vector2 canvasDimensions = canvas.GetComponent<RectTransform>().sizeDelta;

            Vector2 rel = new Vector2(pos.x / canvasDimensions.x, pos.y / canvasDimensions.y);
            Vector2 posInUiCanvas = new Vector2(rel.x * uiCanvasDimensions.x, rel.y * uiCanvasDimensions.y) - uiCanvasDimensions / 2;

            visibleHandCardPreviews.Add(card, UiCardPreviewManager.Instance.ShowCardPreview(posInUiCanvas));
        }
        
    }
    public void HideCardTooltip(GameObject hoveredCard)
    {
        if(visibleHandCardPreviews.ContainsKey(hoveredCard))
        {
            Debug.Log("Hiding tooltip");
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
