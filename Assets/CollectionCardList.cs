using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionCardList : MonoBehaviour
{
    [SerializeField]
    private GameObject cardContainerPrefab;
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

}
