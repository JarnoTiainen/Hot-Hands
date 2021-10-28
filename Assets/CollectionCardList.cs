using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectionCardList : MonoBehaviour
{
    [SerializeField]
    private GameObject cardContainerPrefab;
    [SerializeField]
    private GameObject container3DPrefab;
    [SerializeField]
    private int cardsPerPage = 8;
    public int currentPage;
    public int totalPages;
    public List<Card> cards = new List<Card>();


    public void PopulateList()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in cards)
        {
            GameObject newContainer = Instantiate(cardContainerPrefab) as GameObject;
            newContainer.SetActive(true);

            Transform newCard = newContainer.transform.Find("Card");
            newCard.GetComponent<CollectionCard>().card = card;

            newContainer.transform.SetParent(gameObject.transform, false);
        }
    }

    public void PopulatePage(int page)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        totalPages = Mathf.CeilToInt(cards.Count / cardsPerPage) + 1;

        if(cards.Count <= cardsPerPage)
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
            int startIndex = (page - 1) * cardsPerPage;

            if(page == totalPages)
            {
                for (int i = startIndex; cards.Count > i; i++)
                {
                    Card card = cards[i];
                    GameObject container3D = Instantiate(container3DPrefab) as GameObject;
                    container3D.SetActive(true);
                    container3D.transform.SetParent(gameObject.transform, false);

                    container3D.GetComponent<CollectionCardContainer>().card = card;
                    container3D.GetComponent<CollectionCardContainer>().InstantiateCard();
                }
            }
            else
            {
                for (int i = startIndex; (startIndex + cardsPerPage) > i; i++)
                {
                    Card card = cards[i];
                    GameObject container3D = Instantiate(container3DPrefab) as GameObject;
                    container3D.SetActive(true);
                    container3D.transform.SetParent(gameObject.transform, false);

                    container3D.GetComponent<CollectionCardContainer>().card = card;
                    container3D.GetComponent<CollectionCardContainer>().InstantiateCard();
                }
            }
            currentPage = page;
        }



    }
}
