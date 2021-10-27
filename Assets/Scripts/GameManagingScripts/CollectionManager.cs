using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    private CardList resourcesCardList;


    public Toggle[] cardListToggles;
    public GameObject[] cardLists;
    public GameObject cardListViewport;



    void Start()
    {
        resourcesCardList = Resources.Load("Card List") as CardList;

        for (int i = 0; cardLists.Length > i; i++)
        {
            SetCardLists(i);
        }
    }

    public void SetCardLists(int i)
    {
        switch (i)
        {
            case 0:
                foreach(CardList.ListCard listCard in resourcesCardList.allCards)
                {
                    cardLists[0].GetComponent<CollectionCardList>().cards.Add(listCard.card);
                }
                break;

            case 1:
                cardLists[1].GetComponent<CollectionCardList>().cards = resourcesCardList.playerDeck;
                break;

            default:
                break;
        }

        cardLists[i].GetComponent<CollectionCardList>().PopulateList();
    }


    public void SelectCardListToShow(int selection)
    {
        for(int i = 0; cardLists.Length > i; i++)
        {
            cardLists[i].SetActive(false);
        }

        if (cardListToggles[selection].isOn)
        {
            cardLists[selection].SetActive(true);
            cardListViewport.GetComponent<ScrollRect>().content = cardLists[selection].GetComponent<RectTransform>();
        }
    }


}
