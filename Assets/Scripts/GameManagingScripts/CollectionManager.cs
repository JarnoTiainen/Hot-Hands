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
    [SerializeField]
    private int activeList = 0;
    [SerializeField]
    private GameObject pageText;


    void Start()
    {
        resourcesCardList = Resources.Load("Card List") as CardList;

        for (int i = 0; cardLists.Length > i; i++)
        {
            SetCardLists(i);
        }
        UpdatePageText();
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
                Debug.LogError("Collection card list doesn't exist.");
                break;
        }
        cardLists[i].GetComponent<CollectionCardList>().PopulatePage(1);
    }


    public void ChangeActiveCardList(int selection)
    {
        for(int i = 0; cardLists.Length > i; i++)
        {
            cardLists[i].SetActive(false);
        }

        if (cardListToggles[selection].isOn)
        {
            cardLists[selection].SetActive(true);
            activeList = selection;
        }
        UpdatePageText();
    }

    public void ChangePage(int i)
    {
        CollectionCardList list = cardLists[activeList].GetComponent<CollectionCardList>();

        if(list.currentPage == 1 && i == -1)
        {
            return;
        }
        if(list.currentPage == list.totalPages && i == 1)
        {
            return;
        }
        list.PopulatePage(list.currentPage + i);
        UpdatePageText();
    }

    private void UpdatePageText()
    {
        CollectionCardList list = cardLists[activeList].GetComponent<CollectionCardList>();
        pageText.GetComponent<TextMeshProUGUI>().text = (list.currentPage) + "/" + list.totalPages;

    }
}
