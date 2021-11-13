using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectionCardList : MonoBehaviour
{
    [SerializeField] private GameObject container3DPrefab;
    [SerializeField] private int calculatedCardsPerPage;
    public int currentPage;
    public int totalPages;
    public List<Card> cards = new List<Card>();

    private void CalculateCardsPerPage()
    {
        GridLayoutGroup grid = gameObject.GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = gameObject.transform.parent.GetComponent<RectTransform>();

        int perRowWithoutSpacing = Mathf.FloorToInt(rectTransform.rect.width / grid.cellSize.x);
        int perColumnWithoutSpacing = Mathf.FloorToInt(rectTransform.rect.height / grid.cellSize.y);

        float cardsPlusSpacingWidth = (perRowWithoutSpacing * grid.cellSize.x) + ((perRowWithoutSpacing - 1) * grid.spacing.x) + grid.padding.left + grid.padding.right;
        float cardsPlusSpacingHeight = (perColumnWithoutSpacing * grid.cellSize.y) + ((perColumnWithoutSpacing - 1) * grid.spacing.y) + grid.padding.top + grid.padding.bottom;

        if (cardsPlusSpacingWidth > rectTransform.rect.width)
        {
            if(cardsPlusSpacingHeight > rectTransform.rect.height)
            {
                calculatedCardsPerPage = (perRowWithoutSpacing - 1) * (perColumnWithoutSpacing - 1);
            }
            else
            {
                calculatedCardsPerPage = (perRowWithoutSpacing - 1) * perColumnWithoutSpacing;
            }
        }
        else if (cardsPlusSpacingWidth <= rectTransform.rect.width)
        {
            if(cardsPlusSpacingHeight > rectTransform.rect.height)
            {
                calculatedCardsPerPage = perRowWithoutSpacing * (perColumnWithoutSpacing - 1);
            }
            else
            {
                calculatedCardsPerPage = perRowWithoutSpacing * perColumnWithoutSpacing;
            }
        }
    }

    public void PopulatePage(int page)
    {
        CalculateCardsPerPage();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        totalPages = Mathf.CeilToInt((cards.Count - 1) / calculatedCardsPerPage) + 1;
        if(cards.Count <= calculatedCardsPerPage)
        {
            foreach (Card card in cards)
            {
                GameObject container3D = Instantiate(container3DPrefab) as GameObject;
                container3D.SetActive(true);
                container3D.transform.SetParent(gameObject.transform, false);

                container3D.GetComponent<CollectionCardContainer>().card = card;
                container3D.GetComponent<CollectionCardContainer>().InstantiateCard();
            }
            currentPage = 1;
        }
        else
        {
            int startIndex = (page - 1) * calculatedCardsPerPage;

            if(page == totalPages)
            {
                for (int i = startIndex; cards.Count > i; i++)
                {
                    Card card = cards[i];
                    GameObject container3D = Instantiate(container3DPrefab) as GameObject;
                    container3D.SetActive(true);
                    container3D.transform.SetParent(gameObject.transform, false);

                    CollectionCardContainer containerScript = container3D.GetComponent<CollectionCardContainer>();
                    containerScript.card = card;
                    containerScript.InstantiateCard();
                }
            }
            else
            {
                for (int i = startIndex; (startIndex + calculatedCardsPerPage) > i; i++)
                {
                    Card card = cards[i];
                    GameObject container3D = Instantiate(container3DPrefab) as GameObject;
                    container3D.SetActive(true);
                    container3D.transform.SetParent(gameObject.transform, false);

                    CollectionCardContainer containerScript = container3D.GetComponent<CollectionCardContainer>();
                    containerScript.card = card;
                    containerScript.InstantiateCard();
                }
            }
            currentPage = page;
        }
    }


    public void SortList(SortMethod sortMethod, bool reverse = false)
    {
        switch (sortMethod)
        {
            case SortMethod.Name:
                cards.Sort(SortByName);
                if (reverse) cards.Reverse();
                break;

            case SortMethod.Cost:
                cards.Sort(SortByCost);
                if (reverse) cards.Reverse();
                break;

            case SortMethod.Value:
                cards.Sort(SortByValue);
                if (reverse) cards.Reverse();
                break;

            case SortMethod.RP:
                cards.Sort(SortByRP);
                if (reverse) cards.Reverse();
                break;

            case SortMethod.LP:
                cards.Sort(SortByLP);
                if (reverse) cards.Reverse();
                break;
        }
        PopulatePage(1);
    }

    // Sorting methods below
    public enum SortMethod
    {
        Name,   //0
        Cost,   //1
        Value,  //2
        RP,     //3
        LP      //4
    }

    public int SortByName(Card card1, Card card2)
    {
        return card1.cardName.CompareTo(card2.cardName);
    }

    public int SortByCost(Card card1, Card card2)
    {
        int value = card1.cost.CompareTo(card2.cost);
        if (value != 0)
        {
            return -value;
        }
        else
        {
            return card1.name.CompareTo(card2.name);
        }
    }

    public int SortByValue(Card card1, Card card2)
    {
        int value = card1.value.CompareTo(card2.value);
        if(value != 0)
        {
            return -value;
        }
        else
        {
            return card1.name.CompareTo(card2.name);
        }
    }

    public int SortByRP(Card card1, Card card2)
    {
        int value = card1.rp.CompareTo(card2.rp);
        if (value != 0)
        {
            return -value;
        }
        else
        {
            return card1.name.CompareTo(card2.name);
        }
    }

    public int SortByLP(Card card1, Card card2)
    {
        int value = card1.lp.CompareTo(card2.lp);
        if (value != 0)
        {
            return -value;
        }
        else
        {
            return card1.name.CompareTo(card2.name);
        }
    }
}
