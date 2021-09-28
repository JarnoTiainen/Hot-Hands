using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UiHand : MonoBehaviour
{
    [SerializeField] private List<GameObject> handCards = new List<GameObject>();
    [SerializeField] private GameObject cardBase;
    [SerializeField] private float gapBetweenCards;
    [SerializeField] private float cardScaleInHand = 1;
    [SerializeField] private float hoveredCardScaleMultiplier;
    [SerializeField] private float hoveredCardLiftAmountY;
    [SerializeField] private float hoveredCardLiftAmountZ;
    [SerializeField] float scaleTransitionTime;


    [Button] public void AddNewCard()
    {
        GameObject newCard = InstantiateNewCard();
        handCards.Add(newCard);
        SetNewCardPositions();
    }
    [Button] public void RemoveCard(int CardIndex = 0)
    {
        GameObject removedCard = handCards[CardIndex];
        handCards.Remove(removedCard);
        Destroy(removedCard);
        SetNewCardPositions();
    }

    private GameObject InstantiateNewCard()
    {
        GameObject newCard = Instantiate(cardBase);
        newCard.transform.SetParent(gameObject.transform);
        newCard.transform.localPosition = new Vector3(0,0,0);
        newCard.transform.localScale = new Vector3(cardScaleInHand, cardScaleInHand, cardScaleInHand);
        return newCard;
    }

    private void SetNewCardPositions()
    {
        for(int i = 0; i < handCards.Count; i++)
        {
            if(i == 0)
            {
                float inGameWidth = cardBase.transform.GetChild(0).transform.localScale.x * handCards[i].transform.localScale.x;
                float totalCardsWidth = TotalCardsWidth();
                float cardPosX = -totalCardsWidth / 2 + inGameWidth / 2;
                handCards[i].transform.localPosition = new Vector3(cardPosX, handCards[i].transform.localPosition.y, handCards[i].transform.localPosition.z);
            }
            else
            {
                float newPosX;
                float previousCardPosX = handCards[i - 1].transform.localPosition.x;
                newPosX = previousCardPosX;
                newPosX += cardBase.transform.GetChild(0).transform.localScale.x * handCards[i - 1].transform.localScale.x / 2;
                newPosX += cardBase.transform.GetChild(0).transform.localScale.x * handCards[i].transform.localScale.x / 2;
                newPosX += gapBetweenCards;

                handCards[i].transform.localPosition = new Vector3(newPosX, handCards[i].transform.localPosition.y, handCards[i].transform.localPosition.z);
            }
        }
    }

    private float TotalCardsWidth()
    {
        float totalCardWidth = 0;
        foreach(GameObject card in handCards)
        {
            totalCardWidth += cardBase.transform.GetChild(0).transform.localScale.x * card.transform.localScale.x;
            if (card != handCards[0]) totalCardWidth += gapBetweenCards;
        }
        return totalCardWidth;
    }

    public void IncreaseCardSize(GameObject card)
    {
        card.transform.localPosition += new Vector3(0, hoveredCardLiftAmountY, hoveredCardLiftAmountZ);
        card.transform.localScale = new Vector3(hoveredCardScaleMultiplier, hoveredCardScaleMultiplier, hoveredCardScaleMultiplier);
        SetNewCardPositions();
    }

    private void Update()
    {
        
    }

    public void DecreaseCardSize(GameObject card)
    {
        card.transform.localPosition -= new Vector3(0, hoveredCardLiftAmountY, hoveredCardLiftAmountZ);
        card.transform.localScale = new Vector3(cardScaleInHand, cardScaleInHand, cardScaleInHand);
        SetNewCardPositions();
    }

}
