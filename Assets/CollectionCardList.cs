using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectionCardList : MonoBehaviour
{
    [SerializeField]
    private GameObject cardContainerPrefab;
    [SerializeField]
    private GameObject card3DPrefab;
    [SerializeField]
    private GameObject container3DPrefab;
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


    public void PopulateList3DContainer()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in cards)
        {
            GameObject container3D = Instantiate(container3DPrefab) as GameObject;
            container3D.SetActive(true);
            container3D.transform.SetParent(gameObject.transform, false);

            container3D.GetComponent<CollectionCardContainer>().card = card;
            container3D.GetComponent<CollectionCardContainer>().InstantiateCard();


        }
    }


}
